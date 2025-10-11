# openai-snarky-chat
🧠 A snarky AI chat built with OpenAI, .NET, and React

A full-stack sample using **.NET 9 (Web API)** and **React + TypeScript** to build a chat UI powered by OpenAI’s API — featuring both **non-streaming** and **streaming (real-time)** chat modes.

---

## 🧰 Prerequisites & Setup

### **Backend (.NET 9 API)**

1. **Get an OpenAI API Key**
   - Log into [OpenAI → API Keys](https://platform.openai.com/account/api-keys)
   - Create a new secret key (it starts with `sk-...`).
   - ⚠️ *Note:* You may need to **add at least $5 USD** to your OpenAI Credit balance (the minimum top-up) to enable API access.  
     This amount is usually enough for testing and small-scale experimentation.

2. **Store the API Key securely using User Secrets**
   Run the following from the same folder where your `SnarkyChat.Api.csproj` file is located:

   ```bash
   cd SnarkyChat.Api
   dotnet user-secrets set "OPENAI_API_KEY" "sk-your-key-here"
   dotnet user-secrets list  # verify it shows the key
   ```

3. **Run the API**
   ```bash
   dotnet run --project SnarkyChat.Api
   ```
   - By default, the API runs at **http://localhost:5199** (configured in `launchSettings.json`).
   - You can test it with the included file **`SnarkyChat.Api.http`** in VS Code or Visual Studio to confirm that your key loads correctly.

---

### **Frontend (React Client)**

4. **Install dependencies**
   ```bash
   cd client
   npm install
   ```
   #### ⚙️ React Dev Server (Vite)
   
   The **React client** uses **[Vite](https://vitejs.dev/)** as its development server and build tool.  
   Vite is a modern alternative to Create React App — it provides **instant startup**, **hot module reloading**, and **faster builds** using native ES modules.
   
   Vite launches a local development server (by default at http://localhost:5173) and **automatically refreshes the browser** whenever you change your React files.

5. **Start the dev server**
   ```bash
   npm run dev
   ```
   - Open the printed URL (usually **http://localhost:5173**).
   - The React app will connect to your backend at **http://localhost:5199**.

6. **Chat away 💬**
   - The **“Chat”** tab uses the regular (non-streaming) endpoint.
   - The **“Chat (Streaming)”** tab displays real-time, token-by-token responses.

---


#### 💬 Customizing ChatGPT’s Personality

Inside the backend project (`SnarkyChat.Api`), the class **`LlmClientService`** includes a method called **`GetSystemPrompt()`**.  
This method defines the **system prompt** — the base instruction that tells ChatGPT *how* to behave in all conversations.

By default, it looks something like this:
```csharp
private string GetSystemPrompt() => 
    "You are a helpful assistant with a snarky but friendly tone.";
```
You can customize this text to change the assistant’s behavior or tone.

## 🧱 Project Structure

```
openai-snarky-chat/
├─ SnarkyChat.Api/           # ASP.NET Core 9 Web API
│  ├─ Controllers/
│  │  └─ ChatLlmController.cs    # Handles /api/llm/chat and /api/llm/stream
│  ├─ Services/
│  │  ├─ LlmClientService.cs # LLM chat service
│  ├─ appsettings.json      # using "gpt-4o-mini" model as today, can be updated to a more recent model
│  └─ Program.cs
│
├─ client/                   # React + TypeScript front-end
│  ├─ src/
│  │  ├─ api/client.ts       # API calls to backend (askAsync / streamAsync)
│  │  ├─ components/
│  │  │  ├─ Chat.tsx         # Non-streaming chat UI
│  │  │  └─ ChatStream.tsx   # Streaming chat UI
│  │  ├─ App.tsx             # Tabbed layout (Chat / ChatStream)
│  │  ├─ main.tsx
│  │  └─ styles.css          # some basic css styles
│  ├─ package.json
│  └─ vite.config.ts
│
└─ README.md
```

---

## ✅ Quick Summary

| Step | Action | Purpose |
|------|---------|----------|
| 1 | Create OpenAI API Key (+$5 credit if needed) | Access GPT models |
| 2 | Save with `dotnet user-secrets` | Secure local storage |
| 3 | Run API (`http://localhost:5199`) | Verify backend works |
| 4 | Install Node 20+ & dependencies | Prepare frontend |
| 5 | Run `npm run dev` | Launch React app |
| 6 | Type a prompt → enjoy | Try both chat modes |

---

### ⚡ Notes
- The streaming endpoint uses **Server-Sent Events (SSE)** for real-time token streaming.
- React app is built with **Vite + TypeScript** for fast dev refresh.
- Backend supports environment variable overrides and user-secrets.
- You can easily deploy both projects to Azure or Vercel.

---

**Author:** Marco Lerma  
**Project:** OpenAI Snarky Chat  
**License:** MIT
