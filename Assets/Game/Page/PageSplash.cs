using System.Collections;
using UnityEngine;

public class PageSplash : PageAutoNavigation
{
    public NokiaTextRenderer line1;
    public NokiaTextRenderer line2;

    public override void OnActivate()
    {
        base.OnActivate();

        line1.gameObject.SetActive(false);
        line2.gameObject.SetActive(false);
        StartCoroutine(Run());
    }

    IEnumerator Run()
    {
        yield return new WaitForSeconds(.5f);
        line1.gameObject.SetActive(true);
        yield return line1.AnimateInterval(null, .1f);

        yield return new WaitForSeconds(.5f);
        line2.gameObject.SetActive(true);
        yield return line2.AnimateInterval(null, .1f);

        yield return new WaitForSeconds(1f);
        Skip();
    }

    public void Skip()
    {
        //Debug.Log("Skip");
        StopAllCoroutines();
        pageManager.SetPage("MainMenu");
    }
}
