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
  chat_type: string
  created_by_user_id: number
  status: string
  timezone: string
  admin_id: number
  bet_permission: string
  chat_king_user_id: number | null
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

export interface PredictionOption {
  option_id: number
  prediction_id: number
  option_label: string
  team_id: number | null
  display_order: number
  wager_count: number
  total_points: number
}

export interface PredictionWager {
  wager_id: number
  user_id: number
  option_id: number
  points_wagered: number
  result_status: string
}

export interface PredictionDetail {
  prediction_id: number
  chat_id: number
  created_by_user_id: number
  game_id: number
  title: string
  description: string | null
  prediction_type: string
  status: string
  pot_points: number
  espn_event_id: string | null
  initial_bet_min: number
  initial_bet_max: number
  lock_at: string | null
  resolved_at: string | null
  created_at: string
  options: PredictionOption[]
  wagers?: PredictionWager[]
  resolution?: {
    resolution_id: number
    winning_option_id: number
    notes: string | null
    resolved_at: string
  } | null
}

export interface LeaderboardEntry {
  user_id: number
  username: string
  points_balance: number
  is_king: boolean
}

export interface StrikeInfo {
  user_id: number
  chat_id: number
  strikes_today: number
  max_strikes: number
  locked: boolean
}
