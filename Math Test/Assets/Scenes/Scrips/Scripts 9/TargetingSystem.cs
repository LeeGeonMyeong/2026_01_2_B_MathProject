using UnityEngine;
using UnityEngine.InputSystem;

public class TargetingSystem : MonoBehaviour
{
    public Transform currentTarget;
    public CameraSlerp cameraSlerp;
    public GameObject crosshairUI;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnRightClick(InputValue value)
    {
        if (!value.isPressed) return;

        // 마우스 위치에서 카메라 기준 Ray 생성 (클릭 위치로 광선 발사)
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Raycast로 충돌 체크
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            // 맞은 오브젝트가 Enemy 태그인지 확인
            if (hit.collider.CompareTag("Enemy"))
            {
                //  타게팅 처리
                // 1. 현재 타겟을 클릭한 적으로 설정
                currentTarget = hit.transform;

                // 2. 카메라가 해당 적을 바라보도록 설정
                cameraSlerp.target = currentTarget;

                // 3. 조준선(UI) 활성화
                crosshairUI.SetActive(true);

                Debug.Log("우클릭됨d"); 
            }
        }
        else
        {
            //  초기화 (타겟 해제)

            //  현재 타겟 제거
            currentTarget = null;

            //  카메라 타겟 해제
            cameraSlerp.target = null;

            //  조준선(UI) 비활성화
            crosshairUI.SetActive(false);
        }

    }
}