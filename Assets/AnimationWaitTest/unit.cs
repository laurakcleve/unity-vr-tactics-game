using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class unit : MonoBehaviour {

	Animator animator;
	public bool attackComplete = false;
	public float attackAnimationLength;

	void Awake() {
		animator = GetComponent<Animator>();

        RuntimeAnimatorController ac = animator.runtimeAnimatorController;
        for (int i = 0; i < ac.animationClips.Length; i++) {
            if (ac.animationClips[i].name == "WAIT04") {
                attackAnimationLength = ac.animationClips[i].length;
            }
        }
    }

	void Start () {
		animator.SetTrigger("attack");
		StartCoroutine(WaitForAttack());
		// StartCoroutine(AnotherWait());
	}

	IEnumerator WaitForAttack() {
		// yield return new WaitUntil(() => attackComplete);
		
		// yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length - (animator.GetCurrentAnimatorClipInfo(0)[0].clip.length*(animator.GetCurrentAnimatorStateInfo(0).normalizedTime%1)));
		// yield return new WaitForSeconds(animator.GetCurrentAnimatorClipInfo(0)[0].clip.length);		

		yield return new WaitForSeconds(attackAnimationLength);


		Debug.Log("Attack done");
	}

	IEnumerator AnotherWait() {
		yield return new WaitForSeconds(2f);
		attackComplete = true;
	}



	// void Update() {
	// 	if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attacking")) {
	// 		Debug.Log("attacking");
	// 	}
	// 	else {
	// 		Debug.Log("not attacking");
	// 	}
	// }
	
}
