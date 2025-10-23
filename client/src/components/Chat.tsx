import { useEffect, useRef, useState } from "react";
import { askAsync } from "../api/client";

type Msg = { role: "user" | "assistant"; content: string };

export default function Chat({ temperature }: { temperature: number }) {
  const [messages, setMessages] = useState<Msg[]>([]);
  const [text, setText] = useState("");
  const [busy, setBusy] = useState(false);
  const endRef = useRef<HTMLDivElement>(null);

  useEffect(() => { endRef.current?.scrollIntoView({ behavior: "smooth" }); }, [messages, busy]);

  async function send() {
    const prompt = text.trim();
    if (!prompt || busy) return;

    const newMessages: Msg[] = [...messages, { role: "user", content: prompt }];
    setMessages(newMessages);
    setText("");
    setBusy(true);
    
    try {
      const reply = await askAsync({ 
        userMessage: prompt, 
        messages: newMessages.map(m => ({ role: m.role, content: m.content })),
        temperature: temperature
      });
      setMessages([...newMessages, { role: "assistant", content: reply }]);
    } catch (e: any) {
      setMessages([...newMessages, { role: "assistant", content: `(Error) ${e?.message ?? e}` }]);
    } finally { setBusy(false); }
  }

  function onKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === "Enter" && !e.shiftKey) { e.preventDefault(); void send(); }
  }

  return (
    <div className="chat-shell">
      <div className="chat-header">OpenAI Snarky Chat</div>
      <div className="chat-window">
        {messages.map((m, i) => (
          <div key={i} className={`bubble ${m.role === "user" ? "from-user" : "from-bot"}`}>{m.content}</div>
        ))}
        {busy && <div className="bubble from-bot typing">Thinking…</div>}
        <div ref={endRef} />
      </div>
      <div className="composer">
        <textarea value={text} onChange={e => setText(e.target.value)} onKeyDown={onKeyDown} placeholder="Ask anything…" />
        <button onClick={send} disabled={busy || !text.trim()}>Send</button>
      </div>
    </div>
  );
}
