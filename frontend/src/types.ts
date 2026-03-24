export interface User {
  user_id: number
  username: string
  email: string
  phone_number: string | null
  add_code: string
  profile_image_url: string | null
  lifetime_points: number
}

export interface Chat {
  chat_id: number
  chat_name: string | null
  admin_id: number
  bet_permission: string
  created_at: string
  updated_at: string
}

export interface TickerGame {
  id: string
  league: string
  matchup: string
  score: string
  status: string
}
