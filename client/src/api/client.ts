export type ChatRequest = { 
  userMessage: string; 
  systemPrompt?: string; 
  model?: string; 
  temperature?: number; 
  messages: { role: "user" | "assistant"; content: string; }[] 
};

const BASE = import.meta.env.VITE_API_BASE_URL ?? ""; // e.g. "", "http://localhost:5001"

export async function askAsync(req: ChatRequest): Promise<string> {
  const resp = await fetch(`${BASE}/api/llm/chat`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(req),
  });
  if (!resp.ok) throw new Error(`HTTP ${resp.status}`);
  const data = await resp.json();
  return data.reply as string;
}

export async function streamAsync(
  req: ChatRequest,
  onToken: (t: string) => void,
  onDone?: () => void
): Promise<void> {
  const resp = await fetch(`${BASE}/api/llm/stream`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify(req),
  });
  if (!resp.ok || !resp.body) throw new Error(`Stream error: HTTP ${resp.status}`);

  const reader = resp.body.getReader();
  const decoder = new TextDecoder();
  while (true) {
    const { value, done } = await reader.read();
    if (done) break;
    const chunk = decoder.decode(value, { stream: true });
    for (const line of chunk.split("\n\n")) {
      if (line.startsWith("data: ")) {
        const token = line.slice(6);
        if (token === "[DONE]" || token === "[[STREAM-DONE]]") return onDone?.();
        onToken(token);
      }
    }
  }
  onDone?.();
}
