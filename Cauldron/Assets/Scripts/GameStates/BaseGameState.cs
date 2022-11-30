namespace CauldronCodebase.GameStates
{
    public abstract class BaseGameState
    {
        public abstract void Enter();
        
        //This method is in the contract but it's not used from outside the states.
        //It should be used to clear the state when we leave it from the state machine.
        //That way the state can close its windows when it's switched from by some other part of code.
        public abstract void Exit();
    }
}