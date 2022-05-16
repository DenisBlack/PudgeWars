using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderBoardPlaceController : MonoBehaviour
{
   public TMP_Text NickName;
   public List<PlaceInfo> PlaceInfos = new List<PlaceInfo>();

   public Image Background;
   public Color PlayerColor;
   public Color BotColor;
   
   public void SetData(CharacterData data)
   {
      if(data == null)
         return;

      if (data.IsPlayer)
         Background.color = PlayerColor;
      else 
         Background.color = BotColor;

      NickName.text = data.Score + " - " + data.NickName;
      
   }
   
   [Serializable]
   public class PlaceInfo
   {
      public int Place;
      public Vector3 PanelPosition;
   }
}
