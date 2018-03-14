// all Game States should implement this
public interface IAppState 
{
    void OnEnter();
	
    void Tick();

    void OnLeave();
}
