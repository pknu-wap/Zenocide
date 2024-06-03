using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] TMP_Text textTmp;
    Sequence sequence;
    [SerializeField] float distance = 50f;
    [SerializeField] float lifeTime = 1f;

    private void Start()
    {
        textTmp = GetComponent<TMP_Text>();
        
        // 생성 위치 랜덤하게 주기
    }

    public IEnumerator PrintDamageText(int damage)
    {
        textTmp.text = damage.ToString();

        sequence = DOTween.Sequence()
            .Append(transform.DOLocalMoveY(distance, lifeTime))
            .Join(textTmp.DOFade(0f, lifeTime));

        yield return new WaitForSeconds(lifeTime);

        Destroy(gameObject);
    }
}
