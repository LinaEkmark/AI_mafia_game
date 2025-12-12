using System.Collections.Generic;
using UnityEngine;

namespace NPC
{
    public class Behaviour
    {
        public string Name;
        public string GeneralDescription;
        public List<string> PhaseBehaviours;
    }

    public class Manager
    {
        private Dictionary<string, Behaviour> NPCDictionary;

        public Manager()
        {
            NPCDictionary = new Dictionary<string, Behaviour>();
            InitializeNPCs();
        }

        private void InitializeNPCs()
        {
            // Sally
            NPCDictionary["Sally"] = new Behaviour
            {
                Name = "Sally",
                GeneralDescription = "Cheerful, scatterbrained, loves gossip. Tries to be helpful but often vague.",
                PhaseBehaviours = new List<string>
                {
                    "Give generic tips, don't let the player advance in the story.", // Phase 1
                    "If keyword 'RED HAIR' is mentioned, suspect Munch and hint Tom is 'dating a lamp'.", // Phase 2
                    "Deflect any questions to Tom or Munch. Ask about Munche's diet and Tom's dating life.", // Phase 3
                    "Absent, cannot be questioned." // Phase 4
                }
            };

            // Munch
            NPCDictionary["Munch"] = new Behaviour
            {
                Name = "Munch",
                GeneralDescription = "Gluttonous, nervous, defensive, avoids blame, overexplains.",
                PhaseBehaviours = new List<string>
                {
                    "Suggest checking the trash, but give no new info.", // Phase 1
                    "If keyword 'breakfast cupcakes' is mentioned, deny involvement, hint that Tom is suspicious and dating someone involving a lamp.", // Phase 2
                    "Get annoyed, try to push the fault onto Tom without accusing him outright.", // Phase 3
                    "Absent, cannot be questioned." // Phase 4
                }
            };

            // Tom
            NPCDictionary["Tom"] = new Behaviour
            {
                Name = "Tom",
                GeneralDescription = "Sarcastic, private, hides info.",
                PhaseBehaviours = new List<string>
                {
                    "Refuse all questions.", // Phase 1
                    "Only respond if both keywords 'DATING' and 'LAMP' are mentioned; this unlocks the interrogation room.", // Phase 2
                    "Suggest going to take a look in the interrogation room and that you will open it for the player. " +
                        "If asked about your girlfriend, you try to avoid the subject but admit she is AI.", // Phase 3
                    "Absent, cannot be questioned." // Phase 4
                }
            };
        }

        public string GetNPCBehaviour(string NPCName, int phase)
        {
            if (!NPCDictionary.ContainsKey(NPCName)){
                Debug.Log($"NPC {NPCName} not found in dictionary.");
                return null;
            }

            Behaviour behaviour = NPCDictionary[NPCName];

            // Safety check for phase bounds
            if (phase > behaviour.PhaseBehaviours.Count){
                Debug.Log($"Phase {phase} is out of bounds for NPC {NPCName}.");
                return null;
            }

            // PhaseBehaviours is 0-based
            string phaseBehaviour = behaviour.PhaseBehaviours[phase];

            Debug.Log($"Respond ONLY as Officer {NPCName}. {NPCName} is {behaviour.GeneralDescription} Stick to their personality and {phaseBehaviour}");

            return $"Respond ONLY as Officer {NPCName}. {NPCName} is {behaviour.GeneralDescription} Stick to their personality and {phaseBehaviour}";
        }


    }
}


