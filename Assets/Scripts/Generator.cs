using UnityEngine;
using System.Collections.Generic;

public class Generator : MonoBehaviour
{
    public int detailsAmount;
    public int modulesAmount;
    public int maxFrameLength;
    public int maxFrameWidth;

    //Все, что находится выше, инициализируется в конструкторе Unity

    public List<Vector3> points = new List<Vector3>();
    public List<Vector3> pointsForGuns = new List<Vector3>();
    public void Start()
    {
        GameObject mainFrame = Instantiate<GameObject>(new GameObject(), new Vector3(0, 0, 0), Quaternion.identity);
        mainFrame.AddComponent<MainFrame>();
        GenerateFrame(mainFrame);
        mainFrame.GetComponent<MainFrame>().frameSize = GetMainFrameSize(mainFrame);
        GenerateWheels(mainFrame);
        GenerateCabin(mainFrame);
        GenerateDetails(mainFrame, detailsAmount);
        GenerateModules(mainFrame, modulesAmount);
        GenerateDetailsOnDetails(mainFrame, points);
        GenerateGuns(mainFrame);
    }

    public void GenerateFrame(GameObject mainFrame)
    {
        GameObject framePref = Resources.Load("Frame") as GameObject;
        for (int i = Random.Range(2, maxFrameLength); i >= 0; i--)
        {
            PlaceObject(framePref, new Vector3(i * 0.8f, 0, 0), Quaternion.identity, mainFrame);
            int z = Random.Range(1, maxFrameWidth);
            for (int j = Random.Range(1, maxFrameWidth); j >= 0; j--)
            {
                PlaceObject(framePref, new Vector3(i * 0.8f, 0, j * 0.4f), Quaternion.identity, mainFrame);
                PlaceObject(framePref, new Vector3(i * 0.8f, 0, -j * 0.4f), Quaternion.identity, mainFrame);
            }
        }
    }
    public Vector3 GetMainFrameSize(GameObject mainFrame)
    {
        float maxX = 0f;
        float maxZ = 0f;
        foreach (Frame frame in mainFrame.GetComponentsInChildren<Frame>())
        {
            if (frame.gameObject.transform.position.x > maxX)
                maxX = frame.gameObject.transform.position.x + frame.gameObject.transform.localScale.x / 2;
            if (frame.gameObject.transform.position.z > maxZ)
                maxZ = frame.gameObject.transform.position.z + frame.gameObject.transform.localScale.z / 2;
        }
        return new Vector3(maxX, 0f, maxZ);
    }

    public void GenerateWheels(GameObject mainFrame)
    {
        foreach (Frame frame in mainFrame.GetComponentsInChildren<Frame>())
        {
            GameObject wheelPref = Resources.Load("Wheel") as GameObject;
            Vector3 pos = frame.transform.position;
            if (!IsObjectHere(new Vector3(pos.x, pos.y, pos.z + 0.27f)))
                PlaceObject(wheelPref, new Vector3(pos.x, pos.y, pos.z + 0.26f), Quaternion.Euler(0, 90, 0), frame.gameObject);
            if (!IsObjectHere(new Vector3(pos.x, pos.y, pos.z - 0.27f)))
                PlaceObject(wheelPref, new Vector3(pos.x, pos.y, pos.z - 0.26f), Quaternion.Euler(0, 90, 0), frame.gameObject);
        }
    }
    public void GenerateCabin(GameObject frame)
    {
        GameObject cabinPref = Resources.Load("Capsule") as GameObject;
        Vector3 pos = frame.GetComponent<MainFrame>().frameSize;
        var rnd = Random.Range(cabinPref.transform.localScale.x / 2, frame.transform.localScale.x);
        PlaceObject(cabinPref, new Vector3(rnd, pos.y + cabinPref.transform.localScale.y / 2, 0), Quaternion.Euler(0, 90, 0), frame);
    }

    public void GenerateDetails(GameObject frame, int detailsAmount)
    {
        Vector3 frameScale = frame.GetComponent<MainFrame>().frameSize;
        int stopCount = 0;
        while (detailsAmount > 0 && stopCount < 100)
        {
            GameObject detail = Resources.Load("Detail" + Random.Range(1, 5)) as GameObject;
            Vector3 detailScale = detail.transform.localScale;
            Vector3 rndPoint = new Vector3(Random.Range(-0.5f, frameScale.x - detailScale.x / 2), Random.Range(detailScale.y / 2, detailScale.y + frameScale.y / 1.5f), Random.Range(frameScale.z - detailScale.z / 2 - 0.3f, frameScale.z + detailScale.z / 2));
            if (AllDirsClear(rndPoint, detailScale) && !AllDirsClear(new Vector3(rndPoint.x, rndPoint.y - detailScale.y / 2, rndPoint.z), detailScale))
            {
                PlaceObject(detail, rndPoint, Quaternion.identity, frame);
                AddPointToList(rndPoint, detailScale, "points");
                rndPoint.z = rndPoint.z * (-1);
                detailsAmount--;
                stopCount = 0;
                if (AllDirsClear(rndPoint, detailScale) && !AllDirsClear(new Vector3(rndPoint.x, rndPoint.y - detailScale.y / 2, rndPoint.z), detailScale))
                {
                    PlaceObject(detail, rndPoint, Quaternion.identity, frame);
                    AddPointToList(rndPoint, detailScale, "points");
                    detailsAmount--;
                }
            }
            else
                stopCount++;
        }
    }

    public void GenerateModules(GameObject frame, int modulesAmount)
    {
        GameObject modulePref = Resources.Load("Module") as GameObject;
        Vector3 frameScale = frame.GetComponent<MainFrame>().frameSize;
        int stopCount = 0;
        while (modulesAmount > 0 && stopCount < 100)
        {
            Vector3 rnd = new Vector3(Random.Range(modulePref.transform.localScale.x/2, frameScale.x), frameScale.y + modulePref.transform.localScale.y / 1.5f, Random.Range(-frameScale.z + 0.5f, frameScale.z - 0.5f));
            if (AllDirsClear(rnd, modulePref.transform.localScale))
            {
                PlaceObject(modulePref, rnd, Quaternion.identity, frame);
                modulesAmount--;
                stopCount = 0;
            }
            else
                stopCount++;
        }
    }

    public void GenerateDetailsOnDetails(GameObject frame, List<Vector3> points)
    {
        GameObject detailOnDetail;
        foreach (Vector3 point in points)
        {
            detailOnDetail = Resources.Load("DetailOnDetail" + Random.Range(1, 3)) as GameObject;
            Vector3 detailScale = detailOnDetail.transform.localScale;
            if (AllDirsClear(point, detailScale))
            {
                PlaceObject(detailOnDetail, point, Quaternion.identity, frame);
                AddPointToList(point, detailScale, "WeaponList");
            }
            else
            {
                var a = detailScale.z;
                detailScale.z = detailScale.x;
                detailScale.x = a;
                if (AllDirsClear(point, detailScale))
                {
                    PlaceObject(detailOnDetail, point, Quaternion.Euler(0, 90, 0), frame);
                    AddPointToList(point, detailScale, "WeaponList");
                }
            }
        }
    }

    public bool AllDirsClear(Vector3 rndPoint, Vector3 detailScale)
    {
        return !Physics.CheckBox(rndPoint, new Vector3(detailScale.x / 2 - 0.01f, detailScale.y / 2 - 0.01f, detailScale.z / 2 - 0.01f));
    }

    public void AddPointToList(Vector3 point, Vector3 detailScale, string nameOfList)
    {
        if (nameOfList == "points")
        {
            point.y = point.y + detailScale.y - 0.2f;
            points.Add(point);
        }
        else
        {
            point.y = point.y + detailScale.y - 0.05f;
            pointsForGuns.Add(point);
        }
    }


    public void GenerateGuns(GameObject frame)
    {
        GameObject gun = Resources.Load("Gun") as GameObject;
        foreach(Vector3 point in pointsForGuns)
        {
            gun.transform.localPosition = point;
            if (GunTest(gun))
                PlaceObject(gun, point, Quaternion.Euler(0, 90, 0), frame);
        }
    }

    public bool GunTest(GameObject gameObject)
    {
        bool check = true;
        float pos = gameObject.transform.rotation.y;
        for (int i = 0; i < 45; i+=5)
        {
            pos += i;
            if (!(check = RaycastFromGun(gameObject)))
                return check;
        }
        pos = pos - 45f;
        for (int i = 0; i < 45; i += 5)
        {
            pos -= i;
            if (!(check = RaycastFromGun(gameObject)))
                return check;
        }
        return check;
    }

    public bool RaycastFromGun(GameObject gameObject)
    {    
        RaycastHit hit;
        Vector3 fwd = gameObject.transform.TransformDirection(Vector3.forward);

        return !Physics.Raycast(gameObject.transform.position, fwd, out hit, 5);
    }

    bool IsObjectHere(Vector3 position)
    {
        Collider[] intersecting = Physics.OverlapSphere(position, 0.01f);
        return intersecting.Length != 0;
    }

    public void PlaceObject(GameObject childObject, Vector3 point, Quaternion angle, GameObject parentObject )
    {
        Instantiate<GameObject>(childObject, point, angle).transform.parent = parentObject.transform; ;
    }
}
