using System.Collections;
using System.Collections.Generic;
using FMOD.Studio;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private FMOD.Studio.EventInstance MusicEvent;
    private FMOD.Studio.EventDescription MusicEventDescription;
    private FMOD.Studio.PARAMETER_DESCRIPTION pd;
    FMOD.Studio.PARAMETER_ID parameterID;
    // Start is called before the first frame update
    void Start()
    {
        MusicEvent = FMODUnity.RuntimeManager.CreateInstance("event:/OST/Fight music");
        
        MusicEventDescription = FMODUnity.RuntimeManager.GetEventDescription("event:/OST/Fight music");
        MusicEventDescription.getParameterDescriptionByName("Situation", out pd);
        parameterID = pd.id;
    }


    public void StartMusic()
    {
        MusicEvent.setParameterByID(parameterID, 0);
        MusicEvent.start();
    }

    public void ChangeSituation(int st)
    {
        switch (st)
        {
          case 0:
              MusicEvent.setParameterByID(parameterID, 0);
              break;
          case 1:
              MusicEvent.setParameterByID(parameterID, 1);
              break;
          case 2:
              MusicEvent.setParameterByID(parameterID, 2);
              break;
        }
    }

    public void StopMusic()
    {
        MusicEvent.stop(STOP_MODE.ALLOWFADEOUT);
    }
}
