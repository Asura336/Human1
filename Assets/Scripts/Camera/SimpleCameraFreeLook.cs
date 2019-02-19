using UnityEngine;

public class SimpleCameraFreeLook : MonoBehaviour
{
    Transform selfTransform;
    [Header("_相机应挂在子节点，子节点旋转角可调_")]
    public Camera cameraNode;
    Transform cameraTrans;
    float alpha;  // 横向角, 规定为与 x 轴的夹角
    float sigma;  // 纵向角，规定为与 y 轴的夹角
    float radius;  // 相机跟随人物的实际半径
    public float maxRadius = 10;
    Vector3 orderTrans;  // 保存镜头目标位置
    Vector3 dir = Vector3.back;  // 从被观察点到镜头的矢量
    public bool isControlable = true;

    [Header("_目标的中心点、偏移量、横向和纵向旋转速度(表示为角度)_")]
    public Transform center;
    public Vector3 centerOffset;
    public float alphaSpeed = 360;
    public float sigmaSpeed = 360;
    float _alphaSpeed;
    float _sigmaSpeed;

    Vector3 Center
    {
        get
        {
            return center.position + centerOffset;
        }
    }

    /// <summary>
    /// 控制镜头在水平面上移动
    /// </summary>
    public float HorizontalHandle { get; set; } = 0;
    /// <summary>
    /// 控制镜头在垂直面上移动
    /// </summary>
    public float VerticalHandle { get; set; } = 0;

    [Header("_分别为镜头与 y 轴夹角的最大值和最小值_")]
    public float sigmaMax = 120;
    public float sigmaMin = 30;
    float _sigmaMax = 3.14f;
    float _sigmaMin = 0;

    [Header("_表示移动和旋转的缓冲程度，值越大越不明显_")]
    [SerializeField, Range(0, 1f)] float lerpFollow = 0.25f;
    [SerializeField, Range(0, 1f)] float slerpRotate = 0.25f;

    [Header("_勾选此项时镜头将随地形调整位置，可设置最短跟随半径_")]
    public bool suitLandform = true;
    public float minRadius = 2;
    public LayerMask raycastLayer;

    [Header("_显示鼠标_")]
    [SerializeField] bool _showCursur = true;
    public bool ShowCursur
    {
        get
        {
            return Cursor.visible;
        }
        set
        {
            _showCursur = value;
            Cursor.visible = _showCursur;
        }
    }

    static readonly float toAngle = 180 / Mathf.PI;
    static readonly float toRadian = Mathf.PI / 180;
    public float AlphaAngle
    {
        get
        {
            return alpha * toAngle;
        }
        set
        {
            alpha = value * toRadian;
        }
    }

    void Start()
    {
        selfTransform = transform;
        cameraNode = GetComponentInChildren<Camera>();
        cameraTrans = cameraNode.transform;

        Init();
        _alphaSpeed = alphaSpeed / 180 * Mathf.PI;
        _sigmaSpeed = sigmaSpeed / 180 * Mathf.PI;
        _sigmaMax = Mathf.Min(sigmaMax / 180 * Mathf.PI, _sigmaMax);
        _sigmaMin = Mathf.Max(sigmaMin / 180 * Mathf.PI, _sigmaMin);
        radius = maxRadius;

        ShowCursur = _showCursur;
    }

    private void Update()
    {
        if (!isControlable)
        {
            HorizontalHandle = 0;
            VerticalHandle = 0;
        }

        alpha += HorizontalHandle * _alphaSpeed * Time.deltaTime;
        sigma += VerticalHandle * _sigmaSpeed * Time.deltaTime;
        if (sigma > Mathf.PI) { sigma = Mathf.PI; }
        if (sigma < _sigmaMin) { sigma = _sigmaMin; }
        if (sigma > _sigmaMax) { sigma = _sigmaMax; }
        dir = new Vector3(Mathf.Cos(alpha) * Mathf.Sin(sigma),
                                 Mathf.Cos(sigma),
                                 Mathf.Sin(alpha) * Mathf.Sin(sigma));
    }

    private void FixedUpdate()
    {
        radius = maxRadius;
        if (!suitLandform) { return; }
        Ray ray = new Ray(Center, selfTransform.position - Center);
        if (Physics.Raycast(ray, out RaycastHit hit, maxRadius, raycastLayer))
        {
            radius = Mathf.Min(
                Vector3.Distance(Center, hit.point), maxRadius);
        }        
    }

    private void LateUpdate()
    {
        // 位置跟随
        orderTrans = Center + dir * maxRadius;
        selfTransform.position = Vector3.Lerp(selfTransform.position, orderTrans, lerpFollow);
        // 视角跟随
        var newForward = Quaternion.LookRotation(Center - selfTransform.position);
        selfTransform.rotation = Quaternion.Slerp(selfTransform.rotation, newForward, slerpRotate);

        cameraTrans.position = Vector3.Lerp(Center, selfTransform.position, radius / maxRadius);
    }

    void Init()
    {
        alpha = -Mathf.PI / 2;
        sigma = Mathf.PI / 2;
        orderTrans = Center + dir * maxRadius;
        selfTransform.position = orderTrans;
    }

    /// <summary>
    /// 按当下的 alpha，sigma 角强制切换视角和位置
    /// </summary>
    public void SetPosForce()
    {
        dir = new Vector3(Mathf.Cos(alpha) * Mathf.Sin(sigma),
                                 Mathf.Cos(sigma),
                                 Mathf.Sin(alpha) * Mathf.Sin(sigma));
        orderTrans = Center + dir * maxRadius;
        selfTransform.position = orderTrans;
        var newForward = Quaternion.LookRotation(Center - selfTransform.position);
        selfTransform.rotation = newForward;
    }

    // 镜头转向看到玩家背后的角度
    public void SetAlphaBack()
    {
        AlphaAngle = 270 - center.localEulerAngles.y;
    }
}
