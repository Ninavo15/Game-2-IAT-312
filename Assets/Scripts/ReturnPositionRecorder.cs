using UnityEngine;

// Attached to the player in Scene 3. Called right before leaving for a
// mini-game (poster ripping, car wipe) so Scene 3 can drop the player back at
// the same spot instead of replaying the car intro on return.
public class ReturnPositionRecorder : MonoBehaviour
{
    public void RecordReturnPosition()
    {
        PosterState.ReturnedFromMiniGame = true;
        PosterState.ReturnPosition = transform.position;
    }
}
