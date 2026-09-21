using System.Collections;
using UnityEngine;

public class DonkeyAniamtionsController : MonoBehaviour
{
    //this script contorls the duration the rig aniamtion gets played

    //when the animations should stop
    public float animationLoopTime = 11f;

    //Donkey's rig animation
    public Animator DonekyRigAnimation;

    //Which animation apply to specifc donkey
    public GameObject Donkey;

    public void startDonkeyAniamtions()
    {
        Debug.Log("donkey animation started");

        //Get the Animator attached to this GameObject
        DonekyRigAnimation = GetComponent<Animator>();

        //Get the GameObject this script is attached to
        Donkey = gameObject;

        StopAllCoroutines();
        StartCoroutine(RunDonkeyAnimations());
    }

    IEnumerator RunDonkeyAnimations()
    {
        // Drinking donkey
        if (Donkey.name == "animatedDrinkingDonkey" || Donkey.name == "animatedDrinkingDonkey (1)")
        {
            // Walking layer OFF
            DonekyRigAnimation.SetLayerWeight(0, 0f);

            // Drinking layer ON
            DonekyRigAnimation.SetLayerWeight(1, 1f);

            DonekyRigAnimation.Play("Donkey-rig|donkeyDrinking", 1);
        }

        // Walking donkey
        else if (Donkey.name == "animatedwalkingDonkey")
        {
            // Walking layer ON
            DonekyRigAnimation.SetLayerWeight(0, 1f);

            // Drinking layer OFF
            DonekyRigAnimation.SetLayerWeight(1, 0f);

            DonekyRigAnimation.Play("Donkey-rig|Donkey-rigAction", 0);
        }

        //wait for aniamtions to end
        yield return new WaitForSeconds(animationLoopTime);

        //stop aniamtions
        DonekyRigAnimation.enabled = false;
    }
}