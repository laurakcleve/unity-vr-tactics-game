using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State {

    public virtual void Enter(AIUnit unit) { }
    public virtual void Enter(PlayerUnit unit) { }

    public virtual void Exit(AIUnit unit) { }
    public virtual void Exit(PlayerUnit unit) { }
	
}
