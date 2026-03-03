using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Allows a UI element to be dragged and dropped from and to a container.
/// 
/// Create a subclass for the type you want to be draggable. Then place on
/// the UI element you want to make draggable.
/// 
/// During dragging, the item is reparented to the parent canvas.
/// 
/// After the item is dropped it will be automatically return to the
/// original UI parent. It is the job of components implementing `IDragContainer`,
/// `IDragDestination and `IDragSource` to update the interface after a drag
/// has occurred.
/// </summary>
/// <typeparam name="T">The type that represents the item being dragged.</typeparam>
public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    where T : class
{
    // PRIVATE STATE
    Vector3 startPosition;
    Transform originalParent;
    IDragSource<T> source;

    // CACHED REFERENCES
    Canvas parentCanvas;

    // LIFECYCLE METHODS
    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        source = GetComponentInParent<IDragSource<T>>();
    }

    // PRIVATE
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        // Else won't get the drop event.
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.SetParent(parentCanvas.transform, true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;
        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(originalParent, true);

        IDragDestination<T> container;
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            container = parentCanvas.GetComponent<IDragDestination<T>>();
            if (container == null)
            {
                Debug.Log("Container Null");
            }

        }
        else
        {
            container = GetContainer(eventData);
        }

        if (container != null)
        {
            DropItemIntoContainer(container);
        }


    }

    private IDragDestination<T> GetContainer(PointerEventData eventData)
    {
        if (eventData.pointerEnter)
        {
            var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
            Debug.Log("Container: " + container);

            return container;
        }
        return null;
    }

    private void DropItemIntoContainer(IDragDestination<T> destination)
    {
        //Debug.Log("Dropping item into container: " + destination);
        if (object.ReferenceEquals(destination, source)) 
        {
            //Debug.Log("Dropping into source, doing nothing.");
            return;
        } 

        var destinationContainer = destination as IDragContainer<T>;
        var sourceContainer = source as IDragContainer<T>;

        // Swap won't be possible
        if (destinationContainer == null || sourceContainer == null ||
            destinationContainer.GetItem() == null ||
            object.ReferenceEquals(destinationContainer.GetItem(), sourceContainer.GetItem()))
        {
            AttemptSimpleTransfer(destination);
            return;
        }
        Debug.Log("Attempting swap");

        AttemptSwap(destinationContainer, sourceContainer);
    }

    private void AttemptSwap(IDragContainer<T> destination, IDragContainer<T> source)
    {
        // Provisionally remove item from both sides. 
        var removedSourceNumber = source.GetNumber();
        //Debug.Log("Removed source number: " + removedSourceNumber);
        var removedSourceItem = source.GetItem();
        //Debug.Log("Removed source item: " + removedSourceItem);

        var removedDestinationNumber = destination.GetNumber();
        //Debug.Log("Removed destination number: " + removedDestinationNumber);
        var removedDestinationItem = destination.GetItem();
        //Debug.Log("Removed destination item: " + removedDestinationItem);

        source.RemoveItems(removedSourceNumber);
        //Debug.Log("Removed " + removedSourceNumber + " " + removedSourceItem + " from source");
        destination.RemoveItems(removedDestinationNumber);
        //Debug.Log("Removed " + removedDestinationNumber + " " + removedDestinationItem + " from destination");

        var sourceTakeBackNumber = CalculateTakeBack(removedSourceItem, removedSourceNumber, source, destination);
        //Debug.Log("Source take back number: " + sourceTakeBackNumber);
        var destinationTakeBackNumber = CalculateTakeBack(removedDestinationItem, removedDestinationNumber, destination, source);
        //Debug.Log("Destination take back number: " + destinationTakeBackNumber);

        // Do take backs (if needed)
        if (sourceTakeBackNumber > 0)
        {
            //Debug.Log("Taking back " + sourceTakeBackNumber + " " + removedSourceItem + " to source");
            source.AddItems(removedSourceItem, sourceTakeBackNumber);
            removedSourceNumber -= sourceTakeBackNumber;
        }
        if (destinationTakeBackNumber > 0)
        {
            //Debug.Log("Taking back " + destinationTakeBackNumber + " " + removedDestinationItem + " to destination");
            destination.AddItems(removedDestinationItem, destinationTakeBackNumber);
            removedDestinationNumber -= destinationTakeBackNumber;
        }

        // Abort if we can't do a successful swap
        if (source.MaxAcceptable(removedDestinationItem) < removedDestinationNumber ||
            destination.MaxAcceptable(removedSourceItem) < removedSourceNumber)
        {
            //Debug.Log("Aborting swap, not enough space. Taking back items.");
            destination.AddItems(removedDestinationItem, removedDestinationNumber);
            source.AddItems(removedSourceItem, removedSourceNumber);
            return;
        }

        // Do swaps
        if (removedDestinationNumber > 0)
        {
            source.AddItems(removedDestinationItem, removedDestinationNumber);
            //Debug.Log("Swapped " + removedDestinationNumber + " " + removedDestinationItem + " to source");
        }
        if (removedSourceNumber > 0)
        {
            destination.AddItems(removedSourceItem, removedSourceNumber);
            //Debug.Log("Swapped " + removedSourceNumber + " " + removedSourceItem + " to destination");
        }
    }

    private bool AttemptSimpleTransfer(IDragDestination<T> destination)
    {
        var draggingItem = source.GetItem();
        var draggingNumber = source.GetNumber();

        var acceptable = destination.MaxAcceptable(draggingItem);
        var toTransfer = Mathf.Min(acceptable, draggingNumber);

        if (toTransfer > 0)
        {
            source.RemoveItems(toTransfer);
            destination.AddItems(draggingItem, toTransfer);
            return false;
        }

        return true;
    }

    private int CalculateTakeBack(T removedItem, int removedNumber, IDragContainer<T> removeSource, IDragContainer<T> destination)
    {
        var takeBackNumber = 0;
        var destinationMaxAcceptable = destination.MaxAcceptable(removedItem);

        if (destinationMaxAcceptable < removedNumber)
        {
            takeBackNumber = removedNumber - destinationMaxAcceptable;

            var sourceTakeBackAcceptable = removeSource.MaxAcceptable(removedItem);

            // Abort and reset
            if (sourceTakeBackAcceptable < takeBackNumber)
            {
                return 0;
            }
        }
        return takeBackNumber;
    }
}