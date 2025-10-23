import { useEffect, useRef, useState } from "react";
import { streamAsync } from "../api/client";

type Msg = { role: "user" | "assistant"; content: string };

export default function ChatStream({ temperature }: { temperature: number }) {
  const [messages, setMessages] = useState<Msg[]>([]);
  const [text, setText] = useState("");
  const [busy, setBusy] = useState(false);
  const endRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    endRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, busy]);

  async function send() {
    const prompt = text.trim();
    if (!prompt || busy) return;

    // add user message and placeholder for streaming assistant reply
    //setMessages((m) => [...m, { role: "user", content: prompt }, { role: "assistant", content: "" }]);
    const newMessages: Msg[] = [...messages, { role: "user", content: prompt }];
    setMessages(newMessages);
    setText("");
    setBusy(true);
    const botIndex = messages.length + 1;

    try {
      await streamAsync(
        {
          userMessage: prompt,
          messages: newMessages.map(m => ({ role: m.role, content: m.content })),
          temperature: temperature
        },
        (token) => {
          setMessages(m => {
                if (botIndex < 0 || botIndex >= m.length) return m;        // guard
                const prev = m[botIndex];
                if (!prev) return m;                                       // extra guard
                const updated: Msg = {
                    role: "assistant",                                       // explicit role
                    content: prev.content + token
                };
                const copy = m.slice();
                copy[botIndex] = updated;
                return copy;
            });
        },
        () => setBusy(false)
      );
    } catch (e: any) {
      setBusy(false);
      setMessages((m) => {
        const copy = [...m];
        copy[botIndex] = { role: "assistant", content: `(Stream error) ${e?.message ?? e}` };
        return copy;
      });
    }
  }

  function onKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      void send();
    }
  }

  return (
    <div className="chat-shell">
      <div className="chat-header">Snarky Chat (Streaming)</div>
      <div className="chat-window">
        {messages.map((m, i) => (
          <div key={i} className={`bubble ${m.role === "user" ? "from-user" : "from-bot"}`}>
            {m.content}
          </div>
        ))}
        {busy && <div className="bubble from-bot typing">Typing...</div>}
        <div ref={endRef} />
      </div>

      <div className="composer">
        <textarea
          value={text}
          onChange={(e) => setText(e.target.value)}
          onKeyDown={onKeyDown}
          placeholder="Ask me anything..."
        />
        <button onClick={send} disabled={busy || !text.trim()}>
          Send
        </button>
      </div>
    </div>
  );
}
