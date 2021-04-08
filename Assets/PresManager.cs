using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresManager : MonoBehaviour
{
    [SerializeField] private KeyCode PlayerMovement;

    [SerializeField] private KeyCode AddTurret;

    [SerializeField] private KeyCode AddSniper;

    [SerializeField] private KeyCode AddProbe;

    [SerializeField] private KeyCode AllEnnemiesActivation;

    [SerializeField] private KeyCode LD_Elements;

    [SerializeField] private GameObject[] Groups;

    [SerializeField] private GameObject[] NMIprefabs;

    [SerializeField] private KeyCode Godmode;

    private bool isGod = false;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var group in Groups)
        {
            group.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(PlayerMovement))
        {
            foreach (var group in Groups)
            {
                group.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(AllEnnemiesActivation))
        {
            Groups[0].SetActive(!Groups[0].activeInHierarchy);
        }
        else if (Input.GetKeyDown(LD_Elements))
        {
            Groups[1].SetActive(!Groups[1].activeInHierarchy);
        }
        else if (Input.GetKeyDown(AddTurret))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                Mathf.Infinity))
            {
                Instantiate(NMIprefabs[0], hit.point, Quaternion.identity);
            }
        }
        else if (Input.GetKeyDown(AddSniper))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                Mathf.Infinity))
            {
                Instantiate(NMIprefabs[1], hit.point, Quaternion.identity);
            }
        }
        else if (Input.GetKeyDown(AddProbe))
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit,
                Mathf.Infinity))
            {
                Instantiate(NMIprefabs[2], hit.point, Quaternion.identity);
            }
        }
        else if (Input.GetKeyDown(Godmode))
        {
            isGod = !isGod;
            if (isGod)
            {
                ObjectReferencer.Instance.Avatar_Object.GetComponent<PlayerLife>().SetGodmode();
                ObjectReferencer.Instance.Avatar_Object.GetComponent<EnergieStored>().SetGodmode();
            }
            else
            {
                ObjectReferencer.Instance.Avatar_Object.GetComponent<PlayerLife>().UnsetGodmode();
                ObjectReferencer.Instance.Avatar_Object.GetComponent<EnergieStored>().UnsetGodmode();
            }
            
        }
    }
}
