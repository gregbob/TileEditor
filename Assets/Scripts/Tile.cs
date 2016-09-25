using UnityEngine;
using System.Collections;

[System.Serializable]
public class Tile : MonoBehaviour {

    public Vector2 index;
    public bool canVisit = true;
    public float height = 0;
    public GameObject attachedObj;
    public GameObject raiseTile;
    
    void OnDrawGizmos()
    {
        if (!canVisit)
        {
            Gizmos.color = Color.black;
            Vector3 end = new Vector3(transform.position.x, transform.position.y + transform.localScale.y, transform.position.z);
            Gizmos.DrawLine(transform.position, end);
            Gizmos.DrawWireSphere(end, transform.localScale.x / 10);
        }
 

    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public int GetX()
    {
        return (int)index.x;
    }
    public int GetY()
    {
        return (int)index.y;
    }
    public void RaiseTile(float inc)
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + height, transform.position.z);
        this.height += inc;

        Transform highestTile = FindHighestTile();
        float delta = highestTile.localScale.y / 2 + inc / 2;
        float pos = highestTile.localPosition.y + delta;


        GameObject temp = Resources.Load("defaultTile") as GameObject;
        raiseTile = Instantiate(temp);
        raiseTile.transform.SetParent(transform);
        raiseTile.transform.localPosition = new Vector3(0, pos, 0);
        raiseTile.transform.localScale = new Vector3(transform.localScale.x, inc, transform.localScale.z);
        


    }

    public void LowerTile(float inc)
    {
        Transform top = FindHighestTile();
        // Dont lower past 0
        if (top.position.y - inc < 0)
            return;

        this.height -= inc;
        DestroyImmediate(top.gameObject);
    }

    public void PlaceObject(GameObject toPlace, bool visitable)
    {
        Transform high = FindHighestTile();
        float topOfTile = high.position.y + high.localScale.y / 2 + toPlace.transform.localScale.y / 2;
        Vector3 pos = new Vector3(transform.position.x, topOfTile, transform.position.z);

        GameObject created = Instantiate(toPlace, pos, Quaternion.identity) as GameObject;
        created.transform.SetParent(transform);

        canVisit = visitable;
        attachedObj = created;
    }

    private Transform FindLowestTile()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        Transform min = transform;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].transform.position.y < min.transform.position.y)
            {
                min = children[i];
            }
        }
        return min;
    }
    public Transform FindHighestTile()
    {
        Transform[] children = GetComponentsInChildren<Transform>();
        Transform max = transform;
        for (int i = 0; i < children.Length; i++)
        {
            if (children[i].transform.position.y > max.transform.position.y)
            {
                max = children[i];
            }
        }
        return max;
    }
    public void RemoveObject()
    {
        if (attachedObj != null)
            DestroyImmediate(attachedObj);

        canVisit = true;
        attachedObj = null;
    }

    public static Tile GetTile(GameObject obj)
    {
        Tile tile = null;
        tile = obj.GetComponent<Tile>();
        if (tile != null)
            return tile;
        tile = obj.GetComponentInParent<Tile>();
        if (tile != null)
            return tile;

        return null;
    }
}
