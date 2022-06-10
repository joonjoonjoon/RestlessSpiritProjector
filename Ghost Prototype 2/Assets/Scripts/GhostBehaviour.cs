using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GhostBehaviour : MonoBehaviour {

    public enum states
    {
        disabled,
        world,
        tank,
        resolved
    }
    public states state;
    public SpriteRenderer sprite;
    public Giffer gif;
    public Color color;
    public bool isShowing;
    public bool isShowingWithDelay;
    public int ID;
    public int currentID;


    public Vector2 destination;
    public float angle;
    public float newDestinationCooldown;
    public float speedCustomScale=1;
    public bool allowMovement=true;
    public FakeTransform startvals;

    void Start () {
        startvals = transform.ToFakeTransform(true);

        sprite = GetComponentInChildren<SpriteRenderer>();
        var count = transform.parent.childCount;
        ID = transform.GetSiblingIndex();
        color = GetComponent<Image>().color.SetH(ID/(float)count *360);
        GetComponent<Image>().color = color;

        gif.LoadFromString(GhostsManager.instance.ghostNames[ID]);
        gameObject.name = GhostsManager.instance.ghostNames[ID];

        if (allowMovement)
        {
            ChangeDestination();
            speedCustomScale = transform.GetSiblingIndex() * 0.2f + 1;
            transform.position = GhostsManager.instance.GetRandomPositionWorld().ToVector3().withZ(transform.position.z);
        }
        AppearSlow();
    }
	
	void Update () {
        if (allowMovement)
        {
            Move();
            transform.Rotate(0, 0, (transform.GetSiblingIndex() - transform.parent.childCount/2f) * 5 * Time.deltaTime);
        }
        newDestinationCooldown -= Time.deltaTime;

	}

    public void Show()
    {
        sprite.color = sprite.color.SetA(1);
        isShowing = true;
        Timer.DelayedExecute(() => { this.isShowingWithDelay = true; }, GameManager.instance.signalDelay);
    }

    public void Hide()
    {
        sprite.color = sprite.color.SetA(0);
        isShowing = false;
        Timer.DelayedExecute(() => { this.isShowingWithDelay = false; }, GameManager.instance.signalDelay);
    }

    public void Disable()
    {
        state = states.disabled;
        gameObject.SetActive(false);
        transform.position = Vector3.up * 10000;
        allowMovement = false;
    }
    public void Enable()
    {
        state = states.world;
        gameObject.SetActive(true);
        transform.position = GhostsManager.instance.GetRandomPositionWorld();
    }


    public void GhostClicked()
    {
        if (GameManager.instance.freezeAllDetection) return;
        GhostsManager.instance.CatchGhost(this);
    }
    public void Catch()
    {
        state = states.tank;
        DebugExtension.Blip("CAUGHT ME " + gameObject.name);
    }

    public void Release()
    {
        state = states.world;
        DebugExtension.Blip("RELEASED ME");
    }

    public void Reset()
    {
        //state = states.world;
        if (state == states.world)
            Show();
    }

    public void Resolve()
    {
        state = states.resolved;
        allowMovement = false;
        transform.position = Vector3.up * 10000;
    }

    Tweener appeartweener;
    public void Disappear()
    {
        if (appeartweener != null) appeartweener.Kill();
        appeartweener = DOTween.To(() => sprite.color, x => sprite.color = x, Color.white.SetA(0), 3.5f);
        //transform.DOScale(0.01f, 1.5f);
    }
    public void Appear()
    {
        if (appeartweener != null) appeartweener.Kill();
        appeartweener = DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, 1.5f);
        //transform.DOScale(startvals.scale.x, 1.5f);
    }

    public void AppearSlow()
    {
        if (appeartweener != null) appeartweener.Kill();
        //transform.localScale = Vector3.one * 0.01f;
        //transform.DOScale(startvals.scale.x, 5f);
        sprite.color = Color.white.SetA(0);
        appeartweener= DOTween.To(() => sprite.color, x => sprite.color = x, Color.white, 5);
    }


    public void Move()
    {
        var x = destination.x - transform.position.ToVector2IgnoreZ().x;
        var y = destination.y - transform.position.ToVector2IgnoreZ().y;

        var angleDest = Mathf.Atan2(y, x) * Mathf.Rad2Deg;

        angle = Mathf.LerpAngle(angle, angleDest, GhostsManager.instance.turnSpeed * speedCustomScale * Time.deltaTime);

        var dir = Quaternion.Euler(0,0,angle) * Vector3.right;
        transform.position += dir * GhostsManager.instance.speed * speedCustomScale * Time.deltaTime;
    }

    public void ChangeDestination(bool force=false)
    {
        if (newDestinationCooldown <= 0 || force)
        {
            if (state == states.world)
            {
                destination = GhostsManager.instance.GetRandomPositionWorld();
            }
            else if(state== states.tank)
            {
                destination = GhostsManager.instance.GetRandomPositionTank();
            }
            newDestinationCooldown = Random.Range(0.5f,1.5f);
        }

    }

    void OnDrawGizmos()
    {
        Gizmos.color = color.SetA(1); ;
        Gizmos.DrawWireSphere(destination.ToVector3(), 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + Quaternion.Euler(0,0,angle) * Vector3.right * 10);
    }

}
