---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/i18n/index.ts"
ref: HEAD
---

import i18n from "i18next";
import { initReactI18next } from "react-i18next";

import de from "./locales/de.json";
import en from "./locales/en.json";
import es from "./locales/es.json";
import fr from "./locales/fr.json";
import ko from "./locales/ko.json";
import ptBR from "./locales/pt-BR.json";
import ru from "./locales/ru.json";
import zhCN from "./locales/zh-CN.json";

export const SUPPORTED_LOCALES = [
  { code: "en", label: "English" },
  { code: "de", label: "Deutsch" },
  { code: "fr", label: "Français" },
  { code: "es", label: "Español" },
  { code: "pt-BR", label: "Português (Brasil)" },
  { code: "ru", label: "Русский" },
  { code: "zh-CN", label: "简体中文" },
  { code: "ko", label: "한국어" },
] as const;

export type SupportedLocale = (typeof SUPPORTED_LOCALES)[number]["code"];

const STORAGE_KEY = "blp-converter-locale";

function detectLocale(): SupportedLocale {
  const stored = localStorage.getItem(STORAGE_KEY);
  if (stored && SUPPORTED_LOCALES.some((l) => l.code === stored)) {
    return stored as SupportedLocale;
  }

  const browser = navigator.language;
  const exact = SUPPORTED_LOCALES.find((l) => l.code === browser);
  if (exact) {
    return exact.code;
  }

  const prefix = browser.split("-")[0];
  const byPrefix = SUPPORTED_LOCALES.find((l) => l.code.startsWith(prefix));
  return byPrefix?.code ?? "en";
}

const initialLocale = detectLocale();

void i18n.use(initReactI18next).init({
  resources: {
    en: { translation: en },
    de: { translation: de },
    fr: { translation: fr },
    es: { translation: es },
    "pt-BR": { translation: ptBR },
    ru: { translation: ru },
    "zh-CN": { translation: zhCN },
    ko: { translation: ko },
  },
  lng: initialLocale,
  fallbackLng: "en",
  interpolation: {
    escapeValue: false,
  },
});

// Keep the document language attribute in sync for accessibility tools.
syncDocumentLang(initialLocale);

function syncDocumentLang(locale: SupportedLocale): void {
  if (typeof document !== "undefined") {
    document.documentElement.lang = locale;
  }
}

export function setAppLocale(locale: SupportedLocale): void {
  localStorage.setItem(STORAGE_KEY, locale);
  syncDocumentLang(locale);
  void i18n.changeLanguage(locale);
}

export function getAppLocale(): SupportedLocale {
  return (i18n.language as SupportedLocale) ?? "en";
}

export default i18n;
