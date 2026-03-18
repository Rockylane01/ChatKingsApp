import type { ReactNode } from "react";
import { useMemo, useState } from "react";
import { useToast } from "@/hooks/use-toast";
import { Dialog, DialogClose, DialogContent, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "@/components/ui/dialog";
import { Tabs, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";

type PredictionCategory = "stats" | "points";
type Sport = "football" | "basketball";

function sportLabel(sport: Sport) {
  return sport === "football" ? "Football" : "Basketball";
}

export default function MakePredictionDialog({
  className,
  trigger,
}: {
  className?: string;
  trigger?: ReactNode;
}) {
  const { toast } = useToast();

  const [open, setOpen] = useState(false);
  const [sport, setSport] = useState<Sport>("football");
  const [category, setCategory] = useState<PredictionCategory>("points");
  const [predictionText, setPredictionText] = useState("");
  const [minPoints, setMinPoints] = useState("10");
  const [maxPoints, setMaxPoints] = useState("100");

  const placeholder = useMemo(() => {
    if (category === "points") return "E.g., Over 100 points, Team A wins, etc.";
    return "E.g., QB throws 2+ TDs, Player X 25+ points, etc.";
  }, [category]);

  const handleSubmit = () => {
    toast({
      title: "Prediction (UI only)",
      description: "This screen is wired for design only — backend comes next.",
    });
    setOpen(false);
  };

  return (
    <Dialog open={open} onOpenChange={setOpen}>
      <DialogTrigger asChild>
        {trigger ?? (
          <button
            type="button"
            className={cn(
              "flex w-full items-center justify-center gap-2 rounded-xl border border-ck-gold/30 bg-ck-gold/10 py-3 text-sm font-bold text-ck-gold hover:bg-ck-gold/20 transition-colors",
              className,
            )}
          >
            Make Prediction
          </button>
        )}
      </DialogTrigger>

      <DialogContent className="max-w-[540px] border-border bg-card p-0">
        <DialogHeader className="border-b border-border px-5 py-4">
          <DialogTitle className="text-center font-heading text-lg font-bold">Make a Prediction</DialogTitle>
        </DialogHeader>

        <div className="px-5 py-4">
          <div className="grid gap-4">
            <div className="grid gap-2 sm:grid-cols-[180px_1fr] sm:items-center">
              <Label className="text-sm text-foreground">Select Sport:</Label>
              <Select value={sport} onValueChange={(v) => setSport(v as Sport)}>
                <SelectTrigger className="h-10 bg-background">
                  <SelectValue placeholder="Select sport" />
                </SelectTrigger>
                <SelectContent>
                  <SelectItem value="football">{sportLabel("football")}</SelectItem>
                  <SelectItem value="basketball">{sportLabel("basketball")}</SelectItem>
                </SelectContent>
              </Select>
            </div>

            <div className="border-t border-border pt-4" />

            <div className="grid gap-2 sm:grid-cols-[180px_1fr] sm:items-center">
              <Label className="text-sm text-foreground">Prediction Category:</Label>
              <Tabs value={category} onValueChange={(v) => setCategory(v as PredictionCategory)}>
                <TabsList className="grid w-full grid-cols-2 bg-secondary">
                  <TabsTrigger value="stats" className="data-[state=active]:bg-background">
                    Stats
                  </TabsTrigger>
                  <TabsTrigger value="points" className="data-[state=active]:bg-background">
                    Points
                  </TabsTrigger>
                </TabsList>
              </Tabs>
            </div>

            <div className="border-t border-border pt-4" />

            <div className="grid gap-2 sm:grid-cols-[180px_1fr]">
              <Label htmlFor="predictionText" className="text-sm text-foreground sm:pt-2">
                Enter Your Prediction:
              </Label>
              <Textarea
                id="predictionText"
                value={predictionText}
                onChange={(e) => setPredictionText(e.target.value)}
                placeholder={placeholder}
                className="min-h-11 resize-none bg-background"
              />
            </div>

            <div className="border-t border-border pt-4" />

            <div className="grid gap-3 sm:grid-cols-2">
              <div className="grid gap-2 sm:grid-cols-[180px_1fr] sm:items-center">
                <Label htmlFor="minPoints" className="text-sm text-foreground">
                  Minimum Points to Bet:
                </Label>
                <Input
                  id="minPoints"
                  inputMode="numeric"
                  value={minPoints}
                  onChange={(e) => setMinPoints(e.target.value)}
                  className="h-10 bg-background"
                />
              </div>

              <div className="grid gap-2 sm:grid-cols-[180px_1fr] sm:items-center">
                <Label htmlFor="maxPoints" className="text-sm text-foreground">
                  Maximum Points to Bet:
                </Label>
                <Input
                  id="maxPoints"
                  inputMode="numeric"
                  value={maxPoints}
                  onChange={(e) => setMaxPoints(e.target.value)}
                  className="h-10 bg-background"
                />
              </div>
            </div>
          </div>
        </div>

        <DialogFooter className="border-t border-border bg-background/40 px-5 py-4 sm:flex-row sm:items-center sm:justify-end">
          <DialogClose asChild>
            <Button type="button" variant="secondary" className="h-10 px-6">
              Cancel
            </Button>
          </DialogClose>
          <Button type="button" className="h-10 px-7 font-bold" onClick={handleSubmit}>
            Make Prediction
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
