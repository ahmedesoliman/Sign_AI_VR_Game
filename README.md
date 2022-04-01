# Sign_AI_VR_Game

Sign Language AI VR GAme is a project build usiny Unity & openCvSharp

Implementing a virtual reality system acting as an interface between the Deaf-Muted communities and non-deaf based on American Sign Language (ASL).

#### Technologies used: Unity Game Engine & OpenCVSharp + Unity free package

## Requirements:

- Unity Game Engine
  - Download & install Unity stable version 2019.4.6f1
- Visual Studio Community
  - Download & install Visual Studio 2019 Community Editor
- OpenCVSharp
  - Download free package from unity asset store OpenCVSharp + Unity
- Windows 10+ Operating system platform (64-bit)

## Demo

<gif src= "./demo/01-04-2022-459.gif" width=auto height= auto>
<gif src= "./demo/01-04-2022-461.gif" width=auto height= auto>

## What is the problem:

Communication is a way of sharing our thoughts, ideas, and feelings with others. Verbal and Non-Verbal are the two modes of communication. Usually, everyone communicates with each other verbally. But speech impaired people cannot communicate with each other verbally. Then they communicate with everyone else via sign language which is a Non Verbal mode of communication. Most prominently this method is used by mute and deaf people.

COVID-19 pandemic changed the way we communicate, learn, and receive education virtually. Deaf and mute people face a hard time communicating with other people for daily needs, tasks, and supplies. The deaf-mute people all over the world use sign language as a medium between them and others for communication. However, only people who have gone under special training can understand sign language to communicate with them. This leads to a big gap between the deaf-dumb community and everyone else.

## How Technology can help:

Sign language is a language that consists of body movements, especially of hand and arms, some facial expressions, and some special symbols for alphabets and numbers. We as non-disabled people are not able to decode those sign gestures. Technology can help in decoding the alphabets and translating them into words or sentences. There should be a system that acts as a mediator between mute-def people and everyone else. So the proposed system aims at converting those sign gestures into text/speech that can be understood by everyone. So this Automated Sign To text/speech conversion system helps in decoding those symbols without the need of an expert person who understands the sign language.

## The Idea:

Usually, a sign language interpreter is used by deaf people to seek help for translating their thoughts to all of us. My prototype will help in identifying the alphabet and give the output in a text format. Later these alphabets can be used to form sentences. The model helps the muted people to communicate with everyone and express themselves. This not only makes their life emotionally better but also makes their life easier in the post covid-19 pandemic in communication and seeking help from medical professionals. As well as they become more employable and independent. It also becomes a lot easier for everyone to understand the muted people who would in-turn be able to help them.

# Challenges:

## Capture

- The camera should have resolution of at least 320 x 320
- the frames should be captured at a rate of 30fps
- No image corruption should happen
- A new image should be captured once the system has processed and extraceted the sign from the previous image.

## Train

- Images should be saved as png images for the distance function to work
- The system should have a feature to allow new users to train the letters.
- There should be no overwriting of a letter refrence image with another letter during traning.
- The images should be stored in a subfolder/dataset for maintainance.

## Detection

- Image should be converted into grayscale image and hand should be detected using contour analysis and extraction.
- System is better to be trained before extraction and detection.
- Runing the system in real time using C++ classes std::threads.

## Threshold

- Threshold for the colors is as below:
- BLACK: (0, 0, 0)
- WHITE: (255, 255, 255)
- RED: (0, 0, 255)
- GREEN: (0, 255, 0)
- BLUE: (255, 0, 0)
- YELLOW: (255, 255, 0)

## Future Scope

- Future enhancement of the system can be done through enhancing the system that can detect the hand gesture in real time with a more accuracy mesusres.
- The system can interpret words and stores it in a database or a text file which later can be used to create snectences. The system can also be integrated into other operating systems or devices which will help to convert sign languge to text.
- This project was developed with a limited knowledge of C++ and openCV. But there is unlimetied opprtunities to make it better.

#### ©️®️ @ahmedesoliman
