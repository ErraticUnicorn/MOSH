Grammar Spec:

Signifier => {A..Z}+:


LineNumber => {0,1,2,3,4,5,6,7,8,9}+


Line => Signifier: LineNumber "{AZaz09`~!@#$%^&*()_+=[]\|;:'\",<.>/?{} }+"


Event => {End | Fight | Quest | None}


Transition => [{{Signifier} LineNumber}? {LineNumber | Event} ]+


Interaction => Line & Transition


Dialogue => {Interaction}+




Example:



A: 0 "My name is Inigo Montoya. You killed my father. Prepare to die." & [{B 2} 3], [{B 1} 2], [1]


A: 1 "Have you nothing to say for yourself?" & [End]


A: 2 "You keep using that word. I do not think it means what you think it means." & [1]


A: 3 "I do not mean to pry, but you don't by any chance happen to have six fingers on your right hand?" & [2]



B: 1 "HE DIDN'T FALL? INCONCEIVABLE." & [None]


B: 2 "I... am not left-handed." & [None] 



