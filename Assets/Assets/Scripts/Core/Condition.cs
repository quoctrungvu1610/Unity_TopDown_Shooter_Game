using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Condition
{
    [SerializeField] Disjunction[] and;


    public bool Check(IEnumerable<IPredicateEvaluator> avaluators)
    {
        if (and != null) 
        {
            foreach (Disjunction dis in and)
            {
                if (!dis.Check(avaluators))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    [System.Serializable]
    class Disjunction 
    {
        [SerializeField] Predicate[] or;

        public bool Check(IEnumerable<IPredicateEvaluator> evaluators) 
        {
            if (or != null) 
            {
                foreach (Predicate pred in or)
                {
                    if (pred != null)
                    {
                        if (pred.Check(evaluators))
                        {
                            return true;
                        }
                    }
                }
            }
            
            return false;
        }

    }

    [System.Serializable]
    class Predicate
    {
        [SerializeField] string predicate;
        [SerializeField] string[] parameters;
        [SerializeField] bool negate = false;

        public bool Check(IEnumerable<IPredicateEvaluator> avaluators)
        {
            foreach (var evaluator in avaluators)
            {
                bool? result = evaluator.Evaluate(predicate, parameters);
                if (result == null)
                {
                    continue;
                }

                if (result == negate)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
