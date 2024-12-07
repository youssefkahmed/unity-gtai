using GTAI;
using GTAI.Formations;
using UnityEngine;
using UnityEngine.UI;

namespace GTAI.UI
{
	[SelectionBase]
	public class FormationButton : MonoBehaviour
	{
		[SerializeField] private Formation formation;
		[SerializeField] private Image FormationImage;

		private void OnValidate()
		{
			UpdateIcon();
		}

		private void UpdateIcon()
		{
			if (formation != null && FormationImage != null)
			{
				FormationImage.sprite = formation.Icon;
			}
		}
		
		public void OnClick()
		{
			UIManager.Instance.SetPlayerFormation(formation);
		}
	}
}
