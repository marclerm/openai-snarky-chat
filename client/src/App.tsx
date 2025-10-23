import { useState } from "react";
import Chat from "./components/Chat";           // non-streaming
import ChatStream from "./components/ChatStream"; // streaming
import "./styles.css";

type TabKey = "nonstream" | "stream";

export default function App() {
  const [tab, setTab] = useState<TabKey>("nonstream");
  const [chatTemperature, setChatTemperature] = useState(0.2);

  return (
    <div className="container">
      <h1 className="title">OpenAI Snarky Chat</h1>

      <div className="temp-control" style={{ marginBottom: "1rem" }}>
          <label htmlFor="temp">🧠 How creative you want the response from AI?: <br/>
          <strong>Current: {chatTemperature.toFixed(1)}</strong> </label>
          <input id="temp" type="range" min="0" max="1" step="0.1"
            value={chatTemperature}
            onChange={(e) => setChatTemperature(parseFloat(e.target.value))}
          />
      </div> 

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
        <Chat temperature={chatTemperature} />
      </section>

      <section
        id="panel-stream"
        role="tabpanel"
        aria-labelledby="tab-stream"
        hidden={tab !== "stream"}
        className="tab-panel"
      >
        <ChatStream temperature={chatTemperature} />
      </section>
    </div>
  );
}
