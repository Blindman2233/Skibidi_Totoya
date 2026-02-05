using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace DS.Elements
{
    using Data.Save;
    using Enumerations;
    using Utilities;
    using Windows;

    public class DSSingleChoiceNode : DSNode
    {
        public override void Initialize(string nodeName, DSGraphView dsGraphView, Vector2 position)
        {
            base.Initialize(nodeName, dsGraphView, position);

            DialogueType = DSDialogueType.SingleChoice;

            DSChoiceSaveData choiceData = new DSChoiceSaveData()
            {
                Text = "Next Dialogue"
            };

            Choices.Add(choiceData);
        }

        public override void Draw()
        {
            base.Draw();

            /* OUTPUT CONTAINER */

            foreach (DSChoiceSaveData choice in Choices)
            {
                Port choicePort = this.CreatePort();

                choicePort.userData = choice;
                choicePort.portName = choice.Text;

                TextField choiceTextField = DSElementUtility.CreateTextField(choice.Text, null, callback =>
                {
                    choice.Text = callback.newValue;
                    choicePort.portName = callback.newValue;
                });

                choiceTextField.AddClasses(
                    "ds-node__text-field",
                    "ds-node__text-field__hidden",
                    "ds-node__choice-text-field"
                );

                IntegerField goodnessField = new IntegerField("Goodness")
                {
                    value = choice.GoodnessPointsDelta
                };
                goodnessField.RegisterValueChangedCallback(evt =>
                {
                    choice.GoodnessPointsDelta = evt.newValue;
                });

                choicePort.Add(choiceTextField);
                choicePort.Add(goodnessField);

                outputContainer.Add(choicePort);
            }

            RefreshExpandedState();
        }
    }
}
