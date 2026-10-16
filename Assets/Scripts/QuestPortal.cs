using UnityEngine;

public class QuestPortal : MonoBehaviour
{
    private float proximoAviso;

    void OnTriggerStay2D(Collider2D outro)
    {
        if (outro.GetComponent<QuestPlayer>() == null || Time.time < proximoAviso)
            return;
        proximoAviso = Time.time + 2f;
        QuestSession.Atual.Portal();
    }
}
