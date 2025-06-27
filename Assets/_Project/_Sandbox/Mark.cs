using UnityEngine;

// A targetable object.
[RequireComponent(typeof(Transform))]
public class Mark : MonoBehaviour, ITargetable
{
    private TargetService _targetService;
    [SerializeField]
    private bool _isTargeted;
    // Point where target marker i.e. chevron will display.
    private Transform _markerPoint;

    // TEMP:
    // TODO: object pool for markers
    public GameObject markerPrefab;
    private bool _markerDisplayed;

    private PlayerAim _playerAim;

    public GameObject marker;

    private Boomerang _boomerang;

    // Start is called before the first frame update
    private void Awake()
    {
        // Registar with target service.
        if (GameObject.Find("Target Service") != null)
        {
            _targetService = GameObject.Find("Target Service").GetComponent<TargetService>();
            _targetService.Register(transform);
        }
        else
        {
            Debug.LogWarning(this.gameObject.name + " Cannot locate Target Service.");
        }
        _markerPoint = transform.Find("Marker Point");

        // Find Player object
        if (GameObject.Find("Player") != null)
        {
            _playerAim = GameObject.Find("Player").GetComponent<PlayerAim>();
        }
        else
        {
            Debug.LogWarning($"{this.gameObject.name} can't find Player in scene");
        }

        // Locate Boomerang in scene
        GameObject boomerangObject = GameObject.Find("Boomerang(Clone)");
        if (boomerangObject != null)
        {
            _boomerang = GameObject.Find("Boomerang(Clone)").GetComponent<Boomerang>();
        }
        else
        {
            Debug.LogWarning("Unable to find Boomerang in scene.");
        }
    }

    private void Start()
    {
        marker = Instantiate(markerPrefab);
        HideMarker();
    }

    private void OnEnable()
    {
        if (_playerAim != null)
        {
            //_playerAim.OnPlayerAim += _playerAim_OnPlayerAim;
            _playerAim.OnPlayerAimComplete += _playerAim_OnPlayerAimComplete;
            if (_boomerang != null)
            {
                _boomerang.OnRevolutionComplete += _boomerang_OnRevolutionComplete;
            }
        }
    }

    private void OnDisable()
    {
        if (_playerAim != null)
        {
            //_playerAim.OnPlayerAim -= _playerAim_OnPlayerAim;
            _playerAim.OnPlayerAimComplete -= _playerAim_OnPlayerAimComplete;

            if (_boomerang != null)
            {
                _boomerang.OnRevolutionComplete -= _boomerang_OnRevolutionComplete;
            }
        }
    }

    //private void _playerAim_OnPlayerAim()
    //{
    //    // TODO:...
    //}

    private void _boomerang_OnRevolutionComplete()
    {
        UnTarget();
    }
    private void _playerAim_OnPlayerAimComplete()
    {
        if (!_isTargeted && !_markerDisplayed) { return; }

        UnTarget();
        HideMarker();
    }

    public void Target()
    {
        if (_isTargeted) return;
        //Debug.Log(this.name + " is targetable!");
        _isTargeted = true;
    }

    public void UnTarget()
    {
        if (!_isTargeted) return;
        _isTargeted = false;
    }

    private void Update()
    {
        if (_isTargeted && !_markerDisplayed)
        {
            DisplayMarker();
        }
        else if (!_isTargeted && _markerDisplayed)
        {
            HideMarker();
        }
    }

    // TODO:FIXME should use an event???
    // Display target marker;
    private void DisplayMarker()
    {
        if (markerPrefab != null && _markerPoint != null)
        {
            _markerDisplayed = true;
            //GameObject marker = Instantiate(markerPrefab);
            marker.SetActive(true);
            marker.transform.position = _markerPoint.position;
        };
    }

    // TODO:
    private void HideMarker()
    {
        //Debug.Log("hide marker");
        marker.SetActive(false);
        _markerDisplayed = false;
    }
}
