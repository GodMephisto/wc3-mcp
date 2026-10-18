---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/ui/warcraftcn/example/ToastExample.tsx"
ref: HEAD
---

"use client";
import { useState } from "react";
import { triggerScrollToast } from "@/components/ui/warcraftcn/toast";
import { Button } from "@/components/ui/warcraftcn/button";
import {
    RadioGroup,
    RadioGroupItem,
} from "@/components/ui/warcraftcn/radio-group";
import { Label } from "@/components/ui/warcraftcn/label";

type Faction = "default" | "human" | "orc" | "undead" | "elf";
type Variant = "default" | "success" | "error" | "warning" | "info";
type Position =
    | "top-left"
    | "top-center"
    | "top-right"
    | "bottom-left"
    | "bottom-center"
    | "bottom-right";

const factionQuotes: Record<Faction, string> = {
    human: "Justice shall be done.",
    orc: "Lok'tar ogar!",
    undead: "The dead shall serve.",
    elf: "I am the iron hand of justice.",
    default: "This is warcraft!"
};

const variantFactionMessages: Record<Faction, Record<Variant | "default", string>> = {
    human: {
        success: "Victory for the Alliance!",
        error: "A grave error has occurred.",
        warning: "Proceed with caution",
        info: "The light guides your path.",
        default: factionQuotes.human
    },
    orc: {
        success: "Strength and honor!",
        error: "The spirits are restless!",
        warning: "This could anger the clan.",
        info: "Lok'tar, move forward!",
        default: factionQuotes.orc
    },
    undead: {
        success: "The dead serve us well.",
        error: "You will know endless torment.",
        warning: "Tread lightly, mortal.",
        info: "Frostmourne yearns.",
        default: factionQuotes.undead
    },
    elf: {
        success: "Nature smiles upon you.",
        error: "Something is amiss in the forest.",
        warning: "Beware of the shadows.",
        info: "The goddess watches over us.",
        default: factionQuotes.elf
    },
    default: {
        success: "Operation successful.",
        error: "Something went wrong!",
        warning: "Heed this warning!",
        info: "Informational toast activated!",
        default: factionQuotes.default
    }
};

const factions: { label: string; value: Faction }[] = [
    { label: "Default", value: "default" },
    { label: "Human", value: "human" },
    { label: "Orc", value: "orc" },
    { label: "Night Elf", value: "elf" },
    { label: "Undead", value: "undead" },
];

const positions: { label: string; value: Position }[] = [
    { label: "Top Left", value: "top-left" },
    { label: "Top Center", value: "top-center" },
    { label: "Top Right", value: "top-right" },
    { label: "Bottom Left", value: "bottom-left" },
    { label: "Bottom Center", value: "bottom-center" },
    { label: "Bottom Right", value: "bottom-right" },
];

const variants: { label: string; value: Variant }[] = [
    { label: "Default", value: "default" },
    { label: "Success", value: "success" },
    { label: "Error", value: "error" },
    { label: "Warning", value: "warning" },
    { label: "Info", value: "info" },
];

const ToastExample = () => {
    const [faction, setFaction] = useState<Faction>("default");
    const [position, setPosition] = useState<Position>("top-center");
    const [variant, setVariant] = useState<Variant>("default");

    const handleShowToast = () => {
        const factionMap = variantFactionMessages[faction] ?? variantFactionMessages.default;
        const message = factionMap[variant] ?? factionMap.default ?? "This is toast";

        triggerScrollToast({
            message,
            faction,
            position,
            variant,
        });
    };

    return (
        <div
            className="w-[90vw] md:w-full md:p-2 p-8"
            style={{
                backgroundImage: "url('/warcraftcn/textarea-bg.webp')",
                backgroundSize: "100% 100%"
            }}        >
            <div className="flex flex-col gap-6 p-4 sm:p-10">
                <fieldset className="mb-2 sm:mb-6">
                    <legend className="font-semibold mb-2 sm:mb-3 text-white">Faction</legend>
                    <RadioGroup
                        defaultValue="default"
                        onValueChange={(val: Faction) => setFaction(val)}
                    >
                        <div className="flex flex-col lg:flex-row items-start lg:items-center gap-2 lg:gap-4">
                            {factions.map((f) => (
                                <div className="flex items-center gap-2" key={f.value}>
                                    <RadioGroupItem value={f.value} id={`faction-${f.value}`} />
                                    <Label htmlFor={`faction-${f.value}`}>{f.label}</Label>
                                </div>
                            ))}
                        </div>
                    </RadioGroup>
                </fieldset>

                <fieldset className="mb-2 sm:mb-6">
                    <legend className="font-semibold mb-2 sm:mb-3 text-white">Position</legend>
                    <RadioGroup
                        defaultValue="top-center"
                        onValueChange={(val: Position) => setPosition(val)}
                    >
                        <div className="flex flex-col lg:flex-row items-start lg:items-center gap-2 lg:gap-4">
                            {positions.map((pos) => (
                                <div className="flex items-center gap-2" key={pos.value}>
                                    <RadioGroupItem value={pos.value} id={`position-${pos.value}`} />
                                    <Label htmlFor={`position-${pos.value}`}>{pos.label}</Label>
                                </div>
                            ))}
                        </div>
                    </RadioGroup>
                </fieldset>

                <fieldset className="mb-2 sm:mb-6">
                    <legend className="font-semibold mb-2 sm:mb-3 text-white">Variant</legend>
                    <RadioGroup
                        defaultValue="default"
                        onValueChange={(val: Variant) => setVariant(val)}
                    >
                        <div className="flex flex-col lg:flex-row items-start lg:items-center gap-2 lg:gap-4">
                            {variants.map((v) => (
                                <div className="flex items-center gap-2" key={v.value}>
                                    <RadioGroupItem value={v.value} id={`variant-${v.value}`} />
                                    <Label htmlFor={`variant-${v.value}`}>{v.label}</Label>
                                </div>
                            ))}
                        </div>
                    </RadioGroup>
                </fieldset>
                <div>
                    <Button variant="frame" onClick={handleShowToast} className="w-[200px] md:min-w-xs py-3 px-6">
                        Show Toast
                    </Button>
                </div>
            </div>
        </div>
    );
};

export { ToastExample };