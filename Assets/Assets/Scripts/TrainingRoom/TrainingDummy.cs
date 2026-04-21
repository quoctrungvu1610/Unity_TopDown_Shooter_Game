using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrainingDummy : MonoBehaviour
{   
    private float moveTime = 1f;
    private Vector3 startPostion;
    private Vector3 endPosition;


    public void Setup(float moveTime, Vector3 startPositon, Vector3 endPosition) 
    {
        this.moveTime = moveTime;
        this.startPostion = startPositon;
        this.endPosition = endPosition;

        SetStartPosition(startPositon);
    }

    private void SetStartPosition(Vector3 startPos) 
    {
        this.transform.position = startPos;
    }

    public void StartMoveDummy()
    {
        StartCoroutine(MoveDummy());
    }

    private IEnumerator MoveDummy() 
    {
        float time = 0f;
        while (time < moveTime) 
        {
            time += Time.deltaTime;
            float t = time / moveTime;
            t = Mathf.SmoothStep(0, 1, t);
            this.transform.position = Vector3.Lerp(startPostion, endPosition, t);
            yield return null;
        }
        this.transform.position = endPosition;
        yield return new WaitForSeconds(0.1f);
        //Destroy(this.gameObject);
        ObjectPool.instance.ReturnObject(this.gameObject);
    }
}
