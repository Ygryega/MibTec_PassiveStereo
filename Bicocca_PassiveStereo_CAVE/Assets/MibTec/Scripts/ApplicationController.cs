using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    #region PRIVATE VARIABLES
    private static ApplicationController instance = null;
    private static readonly object padlock = new object();

    private bool startVideo = false;
    #endregion

    #region PUBLIC VARIABLES
    public bool StartVideo
    {
        get => startVideo;
        set => startVideo = value;
    }
    #endregion

    #region SINGLETON
    public static ApplicationController Instance
    {
        get
        {
            if (instance == null)
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new ApplicationController();
                    }
                }
            }
            return instance;
        }
    }
    #endregion

    #region MONOBEHAVIOUR METHODS
    private void Awake()
    {
        InIt();
    }
    #endregion

    #region PRIVATE METHODS
    private void InIt()
    {
        instance = this;
    }
    #endregion
}
