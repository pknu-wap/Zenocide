using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textTmp;
    Sequence sequence;
    [SerializeField] float distance = 50f;
    [SerializeField] float lifeTime = 1f;
    [SerializeField] float randomPositionOffset = 50f;

    public IEnumerator PrintDamageText(int damage)
    {
        textTmp = GetComponent<TextMeshProUGUI>();
        textTmp.text = damage.ToString();
        textTmp.transform.position += (Random.Range(-randomPositionOffset, randomPositionOffset) * new Vector3(1, 0, 0));
        textTmp.transform.position += (Random.Range(0, randomPositionOffset) * new Vector3(0, 1, 0));

        sequence = DOTween.Sequence()
            .Append(transform.DOLocalMoveY(distance, lifeTime))
            .Join(textTmp.DOFade(0f, lifeTime));

        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
