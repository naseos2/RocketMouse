using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneratorScript : MonoBehaviour
{
    // �߰��� �� �������� �����ϴ� �迭
    public GameObject[] availableRooms;
    // ���� ���ӿ� ������ �� ������Ʈ ����Ʈ
    public List<GameObject> currentRooms;

    // ȭ���� ���� ũ��(���� : ����)
    float screenWidthInPoints;
    // �ٴ� ������Ʈ �̸�
    const string floor = "Floor";

    // �߰��� ������Ʈ �������� �����ϴ� �迭
    public GameObject[] availableObjects;
    // ���� ���ӿ� ������ ����, ������ ������Ʈ ����Ʈ
    public List<GameObject> objects;
    // ������Ʈ�� �ּ� ����
    public float objectsMinDistance = 5f;
    // ������Ʈ�� �ִ� ����
    public float objectsMaxDistance = 10f;
    // ������Ʈ Y�� ��ġ �ּ� ��
    public float objectsMinY = -1.4f;
    // ������Ʈ Y�� ��ġ �ִ� ��
    public float objectsMaxY = 1.4f;
    // ������Ʈ �ּ� ȸ����
    public float objectsMinRotation = -45f;
    // ������Ʈ �ִ� ȸ����
    public float objectsMaxRotation = 45f;

    public MouseController mc;

    private void Start()
    {
        // ī�޶��� ������ ���� 2��� ���� ���� ũ�� ���
        float height = 2.0f * Camera.main.orthographicSize;     // height : 6.4
        // ���� ũ�⿡ ȭ�� ������ ���ؼ� ���� ũ�� ���
        screenWidthInPoints = height * Camera.main.aspect;
    }

    private void Update()
    {
        GenerateRoomIfRequired();
        GenerateObjectIfRequired();
    }

    private void AddRoom(float farthestRoomEndX)
    {
        // �� �����յ� �� �ϳ��� �������� ����
        int randomRoomIndex = Random.Range(0, availableRooms.Length);
        // ���õ� �� ������Ʈ�� �߰� 
        GameObject room = (GameObject)Instantiate(availableRooms[randomRoomIndex]);
        // ���� ���� ũ��
        float roomWidth = room.transform.Find(floor).localScale.x;
        // ���� �߾� ��ġ 
        float roomCenter = farthestRoomEndX + roomWidth / 2;
        // ���� ���� ��ġ�� ������Ʈ�� ��ġ ��Ŵ
        room.transform.position = new Vector3(roomCenter, 0, 0);
        // �߰��� ���� ���� �߰��� �渮��Ʈ�� �߰� 
        currentRooms.Add(room);
    }

    private void GenerateRoomIfRequired()
    {
        // ������ ���� ����� �����ϴ� ����Ʈ
        List<GameObject> roomsToRemove = new List<GameObject>();
        // ���� �����ӿ� ���� �������� ���� > �⺻ :  ����
        bool addRooms = true;

        // ���콺 ������Ʈ�� x�� ��ġ
        float playerX = transform.position.x;
        // ������ ���� ���� ��ġ�� ���� (���콺�� ȭ�� �ϳ���ŭ �־����� ����)
        float removeRoomX = playerX - screenWidthInPoints;
        // �߰��� ���� ���� ��ġ�� ���� (���콺�� �������� ȭ�� �Ѱ���ŭ�� ���� �غ��Ѵ�)
        float addRoomX = playerX + screenWidthInPoints;
        // ���� �����ʿ� ��ġ�� ���� ������ �� ��ġ
        float farthesToomEndX = 0f;

        // ���� �߰��� ���� �ϳ��� ó�� 
        foreach(var room in currentRooms)
        {
            // room ������Ʈ�� �ٴ� ������Ʈ�� ã�� ����ũ�⸦ ������
            float roomWidth = room.transform.Find(floor).localScale.x;
            // ���� �߾���ġ���� ���� ũ���� ������ �� ���� �� ��ġ�� ���
            float roomStartX = room.transform.position.x - roomWidth / 2;
            // ���� ���� �� ��ġ���� ���� ũ�⸦ ���� ������ �� ��ġ�� ���
            float roomEndX = roomStartX + roomWidth;

            // ���� ���� �� ��ġ�� �� �߰� ���� ��ġ���� �����ʿ� ������ �� �߰�X 
            if (roomStartX > addRoomX)
                addRooms = false;

            // �� ���� ���� ��ġ���� ���ʿ� �����ϴ� ���� ������ �� ���� ��Ͽ� �߰� 
            if (roomEndX < removeRoomX)
                roomsToRemove.Add(room);

            // ���� ������ ���� ������ �� ��ġ�� �ִ��� �޼ҵ带 �̿��Ͽ� ����
            farthesToomEndX = Mathf.Max(farthesToomEndX, roomEndX);
        }

        // ������ �� ������Ʈ�� �ϳ��� �����ϸ鼭 
        foreach (var room in roomsToRemove)
        {
            // ����Ʈ���� ����
            currentRooms.Remove(room);
            // ������Ʈ ����
            Destroy(room);
        }

        //���� �߰��ؾ� �Ѵٸ� ���� �߰�
        if (addRooms)
            AddRoom(farthesToomEndX);

    }

    private void AddObject(float lastObjectX)
    {
        int randomIndex;
        if (mc.isFever)
        {
            do
            {
                // �߰��� ������Ʈ�� �ε����� �������� ���ϱ�
                // �������� �ȳ��� �� ���� ��� ���� ������ 
                randomIndex = Random.Range(0, availableObjects.Length);
            } while (availableObjects[randomIndex].CompareTag("Laser"));
        }
        else
            randomIndex = Random.Range(0, availableObjects.Length);

        // �������� ���� �ε��� ��ȣ�� ������Ʈ�� ����
        GameObject obj = Instantiate(availableObjects[randomIndex]);
        // ���ο� ������Ʈ�� X�� ��ġ�� ���
        float objectPositionX =
            lastObjectX + Random.Range(objectsMinDistance, objectsMaxDistance);
        // ���ο� ������Ʈ�� Y�� ��ġ�� ���
        float randomY = Random.Range(objectsMinY, objectsMaxY);

        // ���� ��ġ���� ������Ʈ�� ��ġ�� ����
        obj.transform.position = new Vector3(objectPositionX, randomY, 0);

        // �������� ȸ�� �� ���
        float rotation = Random.Range(objectsMinRotation, objectsMaxRotation);

        // ������Ʈ�� ȸ�� �� ����
        obj.transform.rotation = Quaternion.Euler(Vector3.forward * rotation);
        // ������Ʈ ����Ʈ�� �߰�
        objects.Add(obj);

    }

    private void GenerateObjectIfRequired()
    {
        // �÷��̾��� X �� ��ġ
        float playerX = transform.position.x;
        // ������ ������Ʈ�� ���� ��ġ
        float removeObjectX = playerX - screenWidthInPoints;
        // �߰��� ������Ʈ�� ���� ��ġ
        float addObjectX = playerX + screenWidthInPoints;
        // ���� �����ʿ� ��ġ�� ������Ʈ�� X�� ��ġ
        float farthestObjectX = 0;

        // ������ ������Ʈ ����Ʈ
        List<GameObject> objectsToRemove = new List<GameObject>();

        // ���� �߰��Ǿ� �ִ� ������Ʈ���� �ϳ��� ó��
        foreach (var obj in objects)
        {
            // ������Ʈ�� X�� ��ġ
            float objX = obj.transform.position.x;
            // �ִ밪 �񱳷� ���� �����ʿ� ��ġ�� ������Ʈ�� ��ġ�� ����
            farthestObjectX = Mathf.Max(farthestObjectX, objX);

            // ������Ʈ ��ġ�� ���� ���� ��ġ���� �����̸� ������Ʈ ���� ����Ʈ�� �߰�
            if (objX < removeObjectX)
                objectsToRemove.Add(obj);

        }

        // ���� ����Ʈ�� �߰��� ������Ʈ�� ��� ����
        foreach (var obj in objectsToRemove)
        {
            // ����Ʈ���� ����
            objects.Remove(obj);
            // ������Ʈ ����
            Destroy(obj);

        }

        // ���� �����ʿ� ��ġ�� ������Ʈ�� �߰� ���� ��ġ���� �����̸� ���ο� ������Ʈ ����
        if (farthestObjectX < addObjectX)
            AddObject(farthestObjectX);
    }

}
