using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExploreQuest : Quest {
    public string LocationName = "Mysterious Cave";

    public override void StartQuest() {
        base.StartQuest();
        Debug.Log($"{Title}: {Description}. Explore {LocationName}.");
        // Burada, oyuncunun keşfetmesi gereken konumu oyuna ekleyebilirsiniz.
    }

    public void LocationExplored() {
        CompleteQuest();
    }

    public override void CompleteQuest() {
        base.CompleteQuest();
        Debug.Log($"{Title} completed!");
        // Görev tamamlandığında yapılacak işlemler (örneğin, ödül verme)
    }
}