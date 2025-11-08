# Unity Smart Behaviour Agent

This is a sample Unity project integrating chat-based interactions with AI agents. These agents, powered by an LLM, can decide which predefined actions to perform in the game after an interaction with the player.
Approach/Project/Code explanation on my [YouTube channel](https://www.youtube.com/@jays_hub_94)!

## Preview

![image](https://github.com/user-attachments/assets/f2d102aa-5946-4706-b949-bf057d8526f7)

## Requirements

- Unity 2022.3.13f1 (the one I used, but any version should work)
- [Ollama](https://ollama.com) (required for running the LLM locally)

## Installation

1. **Clone the repository:**
   ```sh
   git clone https://github.com/jonathansim94/UnitySmartBehaviourAgent
   ```
2. **Install Ollama**:
   - Follow the official installation guide at [Ollama](https://ollama.com)
  
3. **Get and test target model (in the example, Meta Llama 3.2)**:
   ```sh
   ollama run llama3.2
   ```
   
4. **Run the project**
   - Press `Play` in the Unity editor to start interacting with AI agents clicking on them and sending messages.

## Features

- Chat with AI agents in real-time
- Agents can analyze inputs and trigger predefined game actions (in the example, for the guards: open the door, shoot the player)
- Powered by an LLM running locally with Ollama (so, completely FREE)

## Troubleshooting
If you experience communication issues with the LLM, ensure that the Ollama HTTP server is running. You can verify this using [Postman](https://www.postman.com/).
Also, make sure to set the right address and port foreach agent.

![image](https://github.com/user-attachments/assets/38d1486d-7013-422f-96e0-fd4bb431f7b5)
![image](https://github.com/user-attachments/assets/30067f91-6078-46f8-86bd-9653b3b98f38)

## License
You can use this code for any purpose.
