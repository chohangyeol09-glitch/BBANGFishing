using CHG._02.Script.Agents;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NKT.UI
{
    //체력바. 뒤쪽 바가 늦게 따라와서 방금 깎인 양이 눈에 남는다
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private CHG._02.Script.Agents.Agent agent;

        [Header("UI")]
        [SerializeField] private Image fill;            //현재 체력

        [Header("잔상")]
        [SerializeField] private float delayHold = 0.4f;    //맞고 나서 멈춰 있는 시간
        [SerializeField] private float delaySpeed = 0.8f;   //초당 줄어드는 비율

        [Header("색")]
        [SerializeField] private float lowRatio = 0.3f;
        [SerializeField] private Color normalColor = new Color(0.45f, 0.85f, 0.4f);
        [SerializeField] private Color lowColor = new Color(0.9f, 0.3f, 0.25f);

        private float _shown;        //잔상 바의 현재값
        private float _lastRatio;
        private float _delayTimer;

        private void Start()
        {
            if (agent == null)
            {
                Debug.LogWarning("HealthBarUI : Agent가 연결되지 않았습니다.", this);
                enabled = false;
                return;
            }

            _lastRatio = Ratio();
            _shown = _lastRatio;

            Apply(_lastRatio);
        }

        private void Update()
        {
            float ratio = Ratio();

            //방금 깎였으면 잔상을 잠깐 붙잡아둔다
            if (ratio < _lastRatio) _delayTimer = delayHold;

            _lastRatio = ratio;

            Apply(ratio);
            UpdateDelayed(ratio);
        }

        private float Ratio()
            => agent.MaxHealth > 0f
                ? Mathf.Clamp01(agent.CurrentHealth / agent.MaxHealth)
                : 0f;

        private void Apply(float ratio)
        {
            if (fill != null)
            {
                fill.fillAmount = ratio;
                fill.color = ratio <= lowRatio ? lowColor : normalColor;
            }

        }

        private void UpdateDelayed(float ratio)
        {
            if (ratio >= _shown)
            {
                //회복은 잔상이 기다릴 이유가 없다
                _shown = ratio;
                _delayTimer = 0f;
            }
            else if (_delayTimer > 0f)
            {
                _delayTimer -= Time.deltaTime;
            }
            else
            {
                _shown = Mathf.MoveTowards(_shown, ratio, delaySpeed * Time.deltaTime);
            }
        }
    }
}
