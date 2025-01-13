using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mirror;

public class UIConnection : MonoBehaviour
{
    [Header("Name Input Panel")]
    [SerializeField] private GameObject nameInpPanel;
    [SerializeField] private TMP_InputField inputName;
    [SerializeField] private GameObject nameAlert;
    [SerializeField] private Button nameConfirmBtn;

    [Header("Char Select Panel")]
    [SerializeField] private GameObject charSelectPanel;
    [SerializeField] private int currPlayModelIndex;
    [SerializeField] private GameObject[] playerModels;
    [SerializeField] private Transform modelSpawnPos;
    [SerializeField] private Button nextBtn;
    [SerializeField] private Button prevBtn;
    [SerializeField] private Button selectBtn;

    [Header("Join Room Panel")]
    [SerializeField] private GameObject joinRoomPanel;
    [SerializeField] private TMP_InputField inputIP;
    [SerializeField] private TMP_InputField inputPort;

    [SerializeField] private Button btnServerOnly;
    [SerializeField] private Button btnHost;
    [SerializeField] private Button btnClient;

    //for changing models
    private int[] playerModelLength;
    private int totalModelCount;

    private int currPlayerBaseModelId;
    private int currPlayerModelId;

    private void Start()
    {
        ActivatePanel(nameInpPanel.name);
        // Init the input field with Network Manager's network address.
        inputIP.text = NetworkManager.singleton.networkAddress;
        GetPort();

        RegisterListeners();

        playerModelLength = new int[playerModels.Length];
        // Set up the model lengths
        for (int i = 0; i < playerModels.Length; i++)
        {
            playerModelLength[i] = playerModels[i].transform.GetChild(0).childCount;
            totalModelCount += playerModels[i].transform.GetChild(0).childCount;
        }

        ChangeIndex(0);
        //RegisterClientEvents();
    }
    #region Name Input Panel Functions
    private void CheckName()
    {
        //Debug.Log($"CheckName is called value: {inputName.text}");
        if (string.IsNullOrEmpty(inputName.text))
        {
            nameAlert.SetActive(true);
        }
        else
        {
            LocalPlayerNick.Instance.SetNickname(inputName.text);
            ActivatePanel(charSelectPanel.name);
        }
    }
    #endregion

    #region Char Select Panel
    private void ChangeIndex(int i)
    {
        //Debug.Log($"ChangeIndex is called. param value = {i}");
        currPlayModelIndex += i;

        if (currPlayModelIndex >= totalModelCount)
            currPlayModelIndex = 0;

        if (currPlayModelIndex < 0)
            currPlayModelIndex = totalModelCount-1;

        CalcModelID(currPlayModelIndex);
        ChangeModel();
    }

    private void CalcModelID(int globalIndex)
    {
        int count = 0;

        for (int i = 0; i < playerModelLength.Length; i++)
        {
            count += playerModelLength[i];

            if (globalIndex < count)
            {
                currPlayerBaseModelId = i;
                currPlayerModelId = globalIndex - (count - playerModelLength[i]);
                return;
            }
        }
    }

    private void ChangeModel()
    {
        //Debug.Log($"ChangeModel is called");

        if (modelSpawnPos.childCount > 0)
        {
            Destroy(modelSpawnPos.GetChild(0).gameObject);
        }

        GameObject model = Instantiate(
            playerModels[currPlayerBaseModelId],
            modelSpawnPos.position,
            Quaternion.Euler(0, 180, 0),
            modelSpawnPos);
        GameObject playerModel = model.transform.GetChild(0).GetChild(currPlayerModelId).gameObject;
        playerModel.SetActive(true);
        //Debug.Log($"{currPlayerBaseModelId}: child {currPlayerModelId} has been activated");
    }

    #endregion
    private void RegisterListeners()
    {
        //for name panel
        inputName.onEndEdit.AddListener(delegate { CheckName(); });
        nameConfirmBtn.onClick.AddListener(CheckName);

        //for char selection panel
        nextBtn.onClick.AddListener(()=>ChangeIndex(1));
        prevBtn.onClick.AddListener(()=>ChangeIndex(-1));
        selectBtn.onClick.AddListener(() => 
        {
            LocalPlayerNick.Instance.SetModelId(currPlayerModelId);
            LocalPlayerNick.Instance.SetBaseModelId(currPlayerBaseModelId);
            ActivatePanel(joinRoomPanel.name);
        });

        //for networking panel
        btnServerOnly.onClick.AddListener(OnClickStartServerButton);
        btnHost.onClick.AddListener(OnClickStartHostButton);
        btnClient.onClick.AddListener(OnClickStartClientButton);

        // Add input field listener to update NetworkManager's Network Address
        // when changed.
        inputIP.onValueChanged.AddListener(delegate { OnNetworkAddressChange(); });
        inputPort.onValueChanged.AddListener(delegate { OnPortChange(); });
    }

    private void ActivatePanel(string name)
    {
        nameInpPanel.SetActive(nameInpPanel.name == name);
        charSelectPanel.SetActive(charSelectPanel.name == name);
        joinRoomPanel.SetActive(joinRoomPanel.name == name);
    }

    #region Networking
    private void OnClickStartServerButton()
    {
        NetworkManager.singleton.StartServer();
    }
    private void OnClickStartHostButton()
    {
        NetworkManager.singleton.StartHost();
    }
    private void OnClickStartClientButton()
    {
        NetworkManager.singleton.StartClient();
    }

    private void OnNetworkAddressChange()
    {
        NetworkManager.singleton.networkAddress = inputIP.text;
    }

    private void OnPortChange()
    {
        SetPort(inputPort.text);
    }
    private void SetPort(string _port)
    {
        // only show a port field if we have a port transport
        // we can't have "IP:PORT" in the address field since this only
        // works for IPV4:PORT.
        // for IPV6:PORT it would be misleading since IPV6 contains ":":
        // 2001:0db8:0000:0000:0000:ff00:0042:8329
        if (Transport.active is PortTransport portTransport)
        {
            // use TryParse in case someone tries to enter non-numeric characters
            if (ushort.TryParse(_port, out ushort port))
                portTransport.Port = port;
        }
    }

    private void GetPort()
    {
        if (Transport.active is PortTransport portTransport)
        {
            inputPort.text = portTransport.Port.ToString();
        }
    }

    #endregion
}
