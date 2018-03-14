using System.Collections;

// all Game States should implement this
public interface IAppState 
{
    IEnumerator OnEnter();
	
    void Tick();

    IEnumerator OnLeave();
}
