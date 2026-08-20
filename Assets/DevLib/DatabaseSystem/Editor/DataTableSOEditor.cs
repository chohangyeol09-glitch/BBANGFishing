using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevLib.DatabaseSystem.Runtime;
using GGMLib.DatabaseSystem.Runtime;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace GGMLib.DatabaseSystem.Editor
{
    [UnityEditor.CustomEditor(typeof(DataTableSO))]
    public class DataTableSOEditor : UnityEditor.Editor
    {
        [SerializeField] private VisualTreeAsset editorView;
        
        public override UnityEngine.UIElements.VisualElement CreateInspectorGUI()
        {
            VisualElement root = new VisualElement();

            InspectorElement.FillDefaultInspector(root, serializedObject, this);

            if (editorView == null)
            {
                root.Add(new Label("[Data tableSO Editor] : VisualTree asset이 할당되지 않았습니다."));
                return root;
            }
            
            editorView.CloneTree(root);
            Button validateButton = root.Q<Button>("validate-button");
            ScrollView resultScroll = root.Q<ScrollView>("result-scroll");
            Label resultLabel = root.Q<Label>("result-label");

            validateButton.clicked += () =>
            {
                DataTableSO dataTable = (DataTableSO)target;
                (string message, bool isPassed) = BuildValidationResult(dataTable);

                resultLabel.text = message;
                resultLabel.RemoveFromClassList("result-pass");
                resultLabel.RemoveFromClassList("result-fail");
                resultLabel.AddToClassList(isPassed ? "result-passed" : "result-fail");
                resultScroll.style.display = DisplayStyle.Flex;
            };
            return root;
        }

        private (string message, bool isPassed) BuildValidationResult(DataTableSO dataTable)
        {
            IndexedAsset[] assets = dataTable.assets;

            if (assets == null || assets.Length == 0)
                return ("검증 완료", true);

            StringBuilder sb = new StringBuilder();
            HashSet<IndexedAsset> failedAssets = new HashSet<IndexedAsset>();

            //중복된 아이디를 검사한다.
            var duplicatedIndexGroup = assets
                .GroupBy(a => a.AssetIndex)
                .Where(g => g.Count() > 1);

            foreach (var group in duplicatedIndexGroup)
            {
                sb.Append($"[중복인덱스] : {group.Key}");
                foreach (IndexedAsset asset in group)
                {
                    sb.AppendLine($" {asset.AssetIndex} : {asset.AssetName}");
                    failedAssets.Add(asset);
                }
            }

            IEnumerable<IndexedAsset> blankNameAsset = assets.Where(a
                => string.IsNullOrEmpty(a.AssetName));
            
            var duplicatedNameGroup = assets
                .GroupBy(a => a.AssetName)
                .Where(g => g.Count() > 1);
            
            foreach (var group in duplicatedNameGroup)
            {
                sb.Append($"[같은 이름] : {group.Key}");
                foreach (IndexedAsset asset in group)
                {
                    sb.AppendLine($" {asset.AssetIndex} : {asset.AssetName}");
                    failedAssets.Add(asset);
                }
            }

            bool blankHeaderPrinted = false;
            foreach (IndexedAsset asset in blankNameAsset)
            {
                if (!blankHeaderPrinted) //헤더가 출력되지 않았다면
                {
                    sb.Append("[빈 이름인 에셋들]");
                    blankHeaderPrinted = true;
                }

                sb.AppendLine($" {asset.AssetIndex} : (공백이름)");
                failedAssets.Add(asset);
            }
            
            
            
            if (failedAssets.Count == 0)
                return ("검증 완료", true);

            return (sb.ToString().TrimEnd(), false);

        }

    }
}