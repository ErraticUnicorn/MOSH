Examples of dialogue:

"I've got a bad feeling about this." -> ["What worries you?" -> 2, "You'd better!" -> 3]

"Evidently nothing!" -> [End]

"Let's fight!" -> [Fight]

"Please help me!" -> ["What can I do?" -> 2, "Go away." -> End]

"My dog is stuck in that tree! Get him down for me." -> [Quest #4]


(?<line>^"[^"]+") -> \[(?<response>"[^"]+" -> (\d+|End|Fight)(, )*)*(?<end>End)??,??(?<quest>Quest #\d+)??,??(?<fight>Fight)??\]
