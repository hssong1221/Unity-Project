using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiftItem : MonoBehaviour
{
    [SerializeField]
    private bool _isLifting;

    public bool isLifting
    {
        get { return _isLifting; }
        set
        {
            _isLifting = value;
        }
    }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _isLifting = false;
    }


}
