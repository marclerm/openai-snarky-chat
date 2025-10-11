import { useState } from "react";
import Chat from "./components/Chat";           // non-streaming
import ChatStream from "./components/ChatStream"; // streaming
import "./styles.css";

type TabKey = "nonstream" | "stream";

export default function App() {
  const [tab, setTab] = useState<TabKey>("nonstream");

  return (
    <div className="container">
      <h1 className="title">OpenAI Snarky Chat</h1>

      <div className="tabs" role="tablist" aria-label="Chat modes">
        <button
          role="tab"
          id="tab-nonstream"
          aria-selected={tab === "nonstream"}
          aria-controls="panel-nonstream"
          className={`tab ${tab === "nonstream" ? "active" : ""}`}
          onClick={() => setTab("nonstream")}
        >
          Chat
        </button>
        <button
          role="tab"
          id="tab-stream"
          aria-selected={tab === "stream"}
          aria-controls="panel-stream"
          className={`tab ${tab === "stream" ? "active" : ""}`}
          onClick={() => setTab("stream")}
        >
          Chat (Streaming)
        </button>
      </div>

      <section
        id="panel-nonstream"
        role="tabpanel"
        aria-labelledby="tab-nonstream"
        hidden={tab !== "nonstream"}
        className="tab-panel"
      >
        <Chat />
      </section>

      <section
        id="panel-stream"
        role="tabpanel"
        aria-labelledby="tab-stream"
        hidden={tab !== "stream"}
        className="tab-panel"
      >
        <ChatStream />
      </section>
    </div>
  );
}
