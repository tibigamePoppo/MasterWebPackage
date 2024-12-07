using System;
using UnityEngine;

[Serializable]
public class CharacterImage {
  public Sprite Sample1;
  public Sprite kuma;
  public Sprite nashi;
  public Sprite shika;
}

[Serializable]
public class Parameter {
  public int roundTime;
  public string title;
}

[Serializable]
public class Story {
  public int storyId;
  public string image;
  public string characterName;
  public string text;
}

[Serializable]
public class Card {
  public int contentId;
  public string name;
  public int popularity;
  public int monetary;
  public int cuteness;
  public string text;
  public string image;
}

[Serializable]
public class MasterData {
  public CharacterImage CharacterImage;
  public Parameter Parameter;
  public Story[] Story;
  public Card[] Card;
}

