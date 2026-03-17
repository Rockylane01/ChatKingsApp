// ChatKings Mock Data

export interface User {
  id: string;
  name: string;
  avatar: string;
  addCode: string;
  strikes: number; // 0-3, global daily
}

export interface ChatMember {
  userId: string;
  points: number;
  isKing: boolean;
  minorityWins: number;
}

export interface Chat {
  id: string;
  name: string;
  members: ChatMember[];
  lastActivity: string;
  activePrediction?: Prediction;
}

export interface PredictionOption {
  id: string;
  text: string;
  wagers: { userId: string; amount: number }[];
}

export interface Prediction {
  id: string;
  question: string;
  options: PredictionOption[];
  createdBy: string;
  minWager: number;
  resolvesAt: string;
  resolved: boolean;
  correctOptionId?: string;
}

export interface Game {
  id: string;
  teamA: { name: string; abbrev: string; color: string };
  teamB: { name: string; abbrev: string; color: string };
  time: string;
  live: boolean;
  scoreA?: number;
  scoreB?: number;
  sport: string;
}

export interface Message {
  id: string;
  userId: string;
  text: string;
  timestamp: string;
  type: "user" | "system" | "prediction";
}

export interface ActivityItem {
  id: string;
  chatName: string;
  question: string;
  yourPick: string;
  result: "won" | "lost" | "pending";
  pointsChange: number;
  date: string;
}

// Current user
export const currentUser: User = {
  id: "u1",
  name: "You",
  avatar: "Y",
  addCode: "48291035",
  strikes: 1,
};

export const users: User[] = [
  currentUser,
  { id: "u2", name: "Alex", avatar: "A", addCode: "73920184", strikes: 0 },
  { id: "u3", name: "Jordan", avatar: "J", addCode: "19384756", strikes: 2 },
  { id: "u4", name: "Sam", avatar: "S", addCode: "56473829", strikes: 3 },
  { id: "u5", name: "Casey", avatar: "C", addCode: "82910374", strikes: 0 },
  { id: "u6", name: "Riley", avatar: "R", addCode: "64738291", strikes: 1 },
  { id: "u7", name: "Morgan", avatar: "M", addCode: "91827364", strikes: 0 },
  { id: "u8", name: "Taylor", avatar: "T", addCode: "37281946", strikes: 1 },
];

export const chats: Chat[] = [
  {
    id: "c1",
    name: "NFL Sunday",
    members: [
      { userId: "u2", points: 1240, isKing: true, minorityWins: 3 },
      { userId: "u1", points: 980, isKing: false, minorityWins: 1 },
      { userId: "u3", points: 870, isKing: false, minorityWins: 2 },
      { userId: "u5", points: 650, isKing: false, minorityWins: 0 },
      { userId: "u7", points: 540, isKing: false, minorityWins: 1 },
    ],
    lastActivity: "2 min ago",
    activePrediction: {
      id: "p1",
      question: "Who wins Bears vs Packers?",
      options: [
        { id: "o1", text: "Bears", wagers: [{ userId: "u2", amount: 50 }, { userId: "u7", amount: 30 }] },
        { id: "o2", text: "Packers", wagers: [{ userId: "u3", amount: 40 }] },
      ],
      createdBy: "u2",
      minWager: 10,
      resolvesAt: "2025-02-12T20:00:00",
      resolved: false,
    },
  },
  {
    id: "c2",
    name: "College Ballers",
    members: [
      { userId: "u1", points: 1410, isKing: true, minorityWins: 4 },
      { userId: "u3", points: 1150, isKing: false, minorityWins: 2 },
      { userId: "u4", points: 890, isKing: false, minorityWins: 1 },
      { userId: "u6", points: 720, isKing: false, minorityWins: 0 },
    ],
    lastActivity: "15 min ago",
    activePrediction: {
      id: "p3",
      question: "Utah vs BYU — total points over/under 52.5?",
      options: [
        { id: "o5", text: "Over 52.5", wagers: [{ userId: "u1", amount: 60 }] },
        { id: "o6", text: "Under 52.5", wagers: [{ userId: "u3", amount: 45 }] },
      ],
      createdBy: "u1",
      minWager: 15,
      resolvesAt: "2025-02-13T19:00:00",
      resolved: false,
    },
  },
  {
    id: "c3",
    name: "Work League",
    members: [
      { userId: "u5", points: 1820, isKing: true, minorityWins: 5 },
      { userId: "u1", points: 1310, isKing: false, minorityWins: 2 },
      { userId: "u6", points: 980, isKing: false, minorityWins: 1 },
      { userId: "u4", points: 750, isKing: false, minorityWins: 0 },
      { userId: "u8", points: 620, isKing: false, minorityWins: 1 },
    ],
    lastActivity: "1 hr ago",
  },
  {
    id: "c4",
    name: "Hoops Heads",
    members: [
      { userId: "u8", points: 960, isKing: true, minorityWins: 2 },
      { userId: "u1", points: 840, isKing: false, minorityWins: 1 },
      { userId: "u7", points: 730, isKing: false, minorityWins: 0 },
      { userId: "u2", points: 680, isKing: false, minorityWins: 1 },
    ],
    lastActivity: "3 hr ago",
  },
];

export const games: Game[] = [
  { id: "g1", teamA: { name: "Bears", abbrev: "CHI", color: "#C83200" }, teamB: { name: "Packers", abbrev: "GB", color: "#203731" }, time: "LIVE", live: true, scoreA: 14, scoreB: 21, sport: "NFL" },
  { id: "g2", teamA: { name: "Utah", abbrev: "UTAH", color: "#CC0000" }, teamB: { name: "BYU", abbrev: "BYU", color: "#002E5D" }, time: "LIVE", live: true, scoreA: 27, scoreB: 31, sport: "NCAAF" },
  { id: "g3", teamA: { name: "Lakers", abbrev: "LAL", color: "#552583" }, teamB: { name: "Celtics", abbrev: "BOS", color: "#007A33" }, time: "7:30 PM", live: false, sport: "NBA" },
  { id: "g4", teamA: { name: "Chiefs", abbrev: "KC", color: "#E31837" }, teamB: { name: "Bills", abbrev: "BUF", color: "#00338D" }, time: "8:15 PM", live: false, sport: "NFL" },
  { id: "g5", teamA: { name: "Cowboys", abbrev: "DAL", color: "#003594" }, teamB: { name: "Eagles", abbrev: "PHI", color: "#004C54" }, time: "Tomorrow", live: false, sport: "NFL" },
  { id: "g6", teamA: { name: "Warriors", abbrev: "GSW", color: "#1D428A" }, teamB: { name: "Suns", abbrev: "PHX", color: "#E56020" }, time: "Tomorrow", live: false, sport: "NBA" },
  { id: "g7", teamA: { name: "Yankees", abbrev: "NYY", color: "#003087" }, teamB: { name: "Red Sox", abbrev: "BOS", color: "#BD3039" }, time: "Fri 7PM", live: false, sport: "MLB" },
  { id: "g8", teamA: { name: "Maple Leafs", abbrev: "TOR", color: "#00205B" }, teamB: { name: "Bruins", abbrev: "BOS", color: "#FFB81C" }, time: "Sat 8PM", live: false, sport: "NHL" },
];

export const chatMessages: Record<string, Message[]> = {
  c1: [
    { id: "m1", userId: "u2", text: "New prediction is up! Who wins tonight?", timestamp: "2:30 PM", type: "system" },
    { id: "m2", userId: "u2", text: "Bears are taking it tonight. Jordan is getting wrecked", timestamp: "2:31 PM", type: "user" },
    { id: "m3", userId: "u3", text: "No shot. Packers by 10 easy. Put your points where your mouth is", timestamp: "2:32 PM", type: "user" },
    { id: "m4", userId: "u1", text: "Let's go! I'm riding with the Bears on this one", timestamp: "2:33 PM", type: "user" },
    { id: "m5", userId: "u5", text: "This is gonna be a close one... not touching it yet", timestamp: "2:34 PM", type: "user" },
    { id: "m6", userId: "u7", text: "Gotta go Bears. All in.", timestamp: "2:36 PM", type: "user" },
    { id: "m7", userId: "u2", text: "That's what I'm talking about! King knows best", timestamp: "2:37 PM", type: "user" },
  ],
  c2: [
    { id: "m20", userId: "u1", text: "Created a new prediction on the Holy War game", timestamp: "1:00 PM", type: "system" },
    { id: "m21", userId: "u3", text: "Under 52.5 all day. Both defenses are legit this year", timestamp: "1:02 PM", type: "user" },
    { id: "m22", userId: "u1", text: "Nah these offenses are cooking. Over hits easy", timestamp: "1:03 PM", type: "user" },
    { id: "m23", userId: "u4", text: "I'm locked out so can't bet but I'd go over too", timestamp: "1:05 PM", type: "user" },
    { id: "m24", userId: "u6", text: "Tough one. Sitting this out for now", timestamp: "1:10 PM", type: "user" },
  ],
  c3: [
    { id: "m30", userId: "u5", text: "No active predictions right now. Taking a break this week", timestamp: "11:00 AM", type: "system" },
    { id: "m31", userId: "u1", text: "Casey you scared to put one up? Been quiet", timestamp: "11:05 AM", type: "user" },
    { id: "m32", userId: "u5", text: "I'm the King. I post when I want", timestamp: "11:06 AM", type: "user" },
    { id: "m33", userId: "u8", text: "lol somebody dethrone Casey already", timestamp: "11:10 AM", type: "user" },
  ],
  c4: [
    { id: "m40", userId: "u8", text: "Lakers vs Celtics tonight. Anyone want to make it interesting?", timestamp: "4:00 PM", type: "user" },
    { id: "m41", userId: "u1", text: "Drop a prediction. Lakers easy", timestamp: "4:05 PM", type: "user" },
    { id: "m42", userId: "u7", text: "Celtics are rolling right now though", timestamp: "4:06 PM", type: "user" },
  ],
};

export const resolvedPredictions: (Prediction & { chatId: string; chatName: string })[] = [
  {
    id: "rp1",
    question: "Chiefs vs Ravens — Who wins?",
    options: [
      { id: "ro1", text: "Chiefs", wagers: [{ userId: "u1", amount: 40 }, { userId: "u2", amount: 50 }] },
      { id: "ro2", text: "Ravens", wagers: [{ userId: "u3", amount: 60 }] },
    ],
    createdBy: "u2",
    minWager: 10,
    resolvesAt: "2025-02-08T20:00:00",
    resolved: true,
    correctOptionId: "ro1",
    chatId: "c1",
    chatName: "NFL Sunday",
  },
  {
    id: "rp2",
    question: "Lakers vs Nuggets — Total points over/under 220?",
    options: [
      { id: "ro3", text: "Over 220", wagers: [{ userId: "u1", amount: 30 }] },
      { id: "ro4", text: "Under 220", wagers: [{ userId: "u8", amount: 25 }, { userId: "u7", amount: 35 }] },
    ],
    createdBy: "u8",
    minWager: 10,
    resolvesAt: "2025-02-07T22:00:00",
    resolved: true,
    correctOptionId: "ro4",
    chatId: "c4",
    chatName: "Hoops Heads",
  },
  {
    id: "rp3",
    question: "Alabama vs Georgia — Spread: Bama +3.5?",
    options: [
      { id: "ro5", text: "Bama covers", wagers: [{ userId: "u3", amount: 50 }] },
      { id: "ro6", text: "Georgia covers", wagers: [{ userId: "u1", amount: 45 }, { userId: "u6", amount: 30 }] },
    ],
    createdBy: "u1",
    minWager: 15,
    resolvesAt: "2025-02-06T19:30:00",
    resolved: true,
    correctOptionId: "ro6",
    chatId: "c2",
    chatName: "College Ballers",
  },
];

export const activityHistory: ActivityItem[] = [
  { id: "a1", chatName: "NFL Sunday", question: "Chiefs vs Ravens — Who wins?", yourPick: "Chiefs", result: "won", pointsChange: 80, date: "Feb 8" },
  { id: "a2", chatName: "Hoops Heads", question: "Lakers vs Nuggets — Over/Under 220?", yourPick: "Over 220", result: "lost", pointsChange: -30, date: "Feb 7" },
  { id: "a3", chatName: "College Ballers", question: "Bama +3.5 — covers?", yourPick: "Georgia covers", result: "won", pointsChange: 65, date: "Feb 6" },
  { id: "a4", chatName: "Work League", question: "49ers vs Seahawks ML?", yourPick: "49ers", result: "won", pointsChange: 45, date: "Feb 5" },
  { id: "a5", chatName: "NFL Sunday", question: "Cowboys vs Eagles — Who wins?", yourPick: "Cowboys", result: "lost", pointsChange: -50, date: "Feb 4" },
  { id: "a6", chatName: "College Ballers", question: "Duke vs UNC — Over/Under 145?", yourPick: "Under 145", result: "won", pointsChange: 55, date: "Feb 3" },
  { id: "a7", chatName: "Hoops Heads", question: "Warriors vs Suns ML?", yourPick: "Warriors", result: "lost", pointsChange: -25, date: "Feb 2" },
  { id: "a8", chatName: "NFL Sunday", question: "Bills vs Dolphins — Spread Bills -6.5?", yourPick: "Bills covers", result: "won", pointsChange: 70, date: "Feb 1" },
];

export const strikeHistory = [
  { date: "Today", strikes: 1, reason: "Changed prediction after lock" },
  { date: "Feb 9", strikes: 0, reason: "Clean day" },
  { date: "Feb 8", strikes: 2, reason: "Late submission x2" },
  { date: "Feb 7", strikes: 0, reason: "Clean day" },
  { date: "Feb 6", strikes: 1, reason: "Disputed resolution" },
];

export function getUserById(id: string): User | undefined {
  return users.find((u) => u.id === id);
}

export function getChatById(id: string): Chat | undefined {
  return chats.find((c) => c.id === id);
}

export function getStrikeColor(strikes: number): string {
  switch (strikes) {
    case 0: return "ck-green";
    case 1: return "ck-yellow";
    case 2: return "ck-orange";
    case 3: return "ck-red";
    default: return "ck-green";
  }
}

export function getStrikeLabel(strikes: number): string {
  switch (strikes) {
    case 0: return "All clear";
    case 1: return "1 strike";
    case 2: return "Careful!";
    case 3: return "Locked out";
    default: return "";
  }
}
