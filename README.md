# ResilientVoiceToText
Recently, i talk with a partner complainning about stability of connexion to cloud when trying to use some cloud services (in his case, it was a  VoiceToText service).

We had a little talk around cloud pattern, connexion glitch and gremlins, environment impact on the felt stablityof the application, especially in an very noisy environment (professionnal show, ...).
We design a different architecture ni order to make the application more resilient,  highly increasing the felt stability and usage efficiency.


## Update
- 6/dec/2017 : support of CustomSpeech (aka CRIS) deployment in the BigAudio sample  : STS url is now configurable see Main() in sample  https://github.com/flyingoverclouds/ResilientVoiceToText/blob/master/BigAudioClientTestApp/Program.cs
