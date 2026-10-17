---
type: vendor-doc
source_repo: "https://github.com/fmajka/vjass"
source_path: "hots2/early-changes-pl.txt"
ref: HEAD
---

HotS2 [Day 1, 06.02.2024]:
- Wygląd Skopania
- Jednostki: fontanna, duży kamień, drzewo
- Dodatki: kuźnia, kapliczka, brama, palisada

HotS2 [Day 2, 07.02.2024]:
- Spisanie informacji o podstawowych jednostkach i wyzwalaczach z HotS1
- Jednostki: Crook, mały kamień
- Zdolności: Interakcja
- Schowanie pasków zdrowia niektórych elementów otoczenia
- Podstawowe zmienne, m.in. Crook[], Players, TOTAL_PLAYERS
- Utrata głodu i pragnienia
- Movement detection system
- Picie wody z fontanny
- Własne stałe gry i nazwy interfejsu

HotS2 [Day 3, 08.02.2024]:
- Nowy folder ze skryptem "C:/dev/war3/hots2/"
- Spisanie informacji o podstawowych itemkach
- Spisanie statystyk kilku mobów
- Spawnowanie zwykłych kwiatków, drop pieniążków
- Drop i jedzenie jabłek (dummy item active)
- Plik "hots.j": leczenie, głód/pragnienie, interakcja, losowa szansa
- Podstawowe ustawienia sojuszu
- Drobne poprawki mapy
- Zalążek Spawner System "Spawner.j"

HotS2 [Day 4, 09.02.2024]:
- Bierna regeneracja zdrowia bohaterów
- Dokończenie spawn system
- Spawnowanie dżdżownic
- System doświadczenia, ważony exp
- Funkcje: CrookAddXP, SetTextTagPlayer, GetRandomCoinId
- Kolory i atrybuty monet, losowe monety z kwiatków

HotS2 [Day 5, 10.02.2024]:
- Poprawki błędów z otrzymywaniem doświadczenia (sojusz z Wioską)
- 4 podstawowe atrybuty: zdolności i powiązane z nimi ulepszenia
- Mechaniki brutalnej siły, część mechanik silnego ciała i filatelistyki
- Wygodne tworzenie kolorowych tekstów: TextTag.j
- Funkcje pomocnicze (Utils.j): zaokrąglanie liczb, %hp jednostki w skali 0-1 [...]
- Damage event, damage detection
- Damage indicators: kolorowe napisy otrzymanych obrażeń
- Napisy pokazujące siłę leczenia
- Komendy pomocniczne: /level, /r (pokazuje poziomy ulepszeń atrybutów)
- Zmodernizowana kopia HotS_Announcer z HotS1: Announcer.j

HotS2 [Day 6, 11.02.2024]:
- Dokończenie i testy Gachi Announcera
- Poprawny spawn rate robali z kamieni
- Model jabłka
- Podnoszenie i upuszczanie robali
- Opis i możliwość spożycia robali
- Przyszłościowe poprawki damage detection system
- Komendy: link start, gachiBASS

Hots2 [Day 7, 13.02.2024]:
- Weapon system z parametrami: dmg, dicesides, range, interval, sound, attachment
- 2 rodzaje broni: Patyk, Pochodnia
- Drop patyków z drzew

HotS2 [Day 8, 15.02.2024]:
- Możliwość przypisania zdolności do broni w WeaponSystem
- Generowanie many przy zadawaniu obrażeń
- Filtrowanie jednostek na obszarze: Combat.j
- Pomocnicze funkcje matematyczne: Math.j
- Zdolność: Empowered Strike (Patyk)
- Efekty i dźwięki zdolności (od teraz może odtwarzać wiele kopii dźwięku na raz!)
- Skalowanie generowanej many i obrażeń zdolności z poziomem Władania Bronią
- Dodatkowe zdrowie za każdy zdobyty poziom

HotS2 [Day 9, 18.02.2024]:
- Jednostki: latarnia, woźny, dummy casterzy
- Podpalanie przeciwników pochodnią
- Zapalanie latarni interakcją trzymając pochodnię w ręku
- Czas świecenia latarni bazowany na timerach
- Funkcje pomocnicze do spawnowanie dummy casterów: Dummy.j
- Tekst announcera po spaleniu przeciwnika

HotS2 [Day 10, 19.02.2024]:
- Możliwość zapalania patyków przy świecących latarniach
- Akcje rozłożone w czasie bazowane na timerach: Timer.j
- Woźny poprawnie zapala wszystkie latarnie
- Nadpisywanie animacji poprzez 0.00s timer
- Cykl życia woźnego

HotS2 [Day 11, 20.02.2024]:
- Przedmioty: BePower
- System buffów: Buff.j
- Pierwszy buff: Sugar Rush
- Zwiększanie zasiegu widzenia po zaekwipowaniu pochodni

HotS2 [Day 12, 21.02.2024]:
- System oświetlenia: Light.j
- System oświetlenia zwiększa zasięg widzenia tylko w nocy
- Light_Event... (wyzwalacz śledzący stan zmiennej rzeczywistej)
- Zdolność disarm uniemożliwiająca jednostce atakowanie
- Drobne poprawki i usprawnienia

HotS2 [Day 13, 22.02.2024]:
- Przedmioty: Soniki
- Jednostki: BeFlower
- Buff regeneracji, poziomy regeneracji
- Picie soników i regen zdrowia
- Poprawiono lewitujące kwiatki

HotS [Day 14, 24.02.2024]:
- Mały updacik ;)
- Dodano Handlarkę Różnościami; sprzedaje BePower i Soniki
- Dodano spawnowanie BeFlowerów
- Jednostki: Wolf; jeszcze nie mogą się spawnować

Bonus [30.05.2024]:
- Przeniesiono skrypty do innego folderu i zmieniono ścieżki na relatywne

HotS [Day 15, 28.07.2024]:
- Wielki powrót?!
- Nieznaczny rewrite skryptów: HOTS.j, Combat.j, Dummy.j
- Działający spell damage tag
- Spell patyka slowuje cele

HotS [Day 16, 29.07.2024]:
- Planowanie terenu okolic Skopania
- Zalążek terenu pomiędzy Skopaniem i Dżunglą

HotS [Day 17, 30.07.2024]:
- Praca nad terenem: cmentarz, równina, las

HotS [Day 18, 31.07.2024]:
- Dalsza praca nad terenem lasu

HotS [Day 19, 02.08.2024]:
- Teren na wschód od Skopania (nad i pod drogą)
- Dodano spawnowanie wilczków
- Kwiaty pojawiają się na obszarze całej mapy
- Dorzucono parę kamieni

HotS [Day 20, 05.08.2024]:
- Przeportowano bazę dla Drewnianego Sztyletu i Katany
- Po wielu ciągłych zmianach w końcu dodano ProjectileSystem
- Pierwszy działający pocisk: PROJECTILE_DAGGER
- Backstab dla sztyletu
- Dodawanie i usuwanie animation tagów dla WeaponSystem

HotS [Day 21, 06.08.2024]:
- Poszerzono możliwości ProjectileSystem o dashowanie się jednostek
- Dodano blokowanie ciosów
- Działające umiejętności katany: Empowered Charge, Deflect, Counter Strike

HotS [Day 22, 08.08.2024]:
- Dodano pasywkę katany
- Wykrzyknik przy krytycznym ciosie (backstab, kata)

HotS [Day 23, 09.08.2024]:
- Nowy system: KnockbackSystem

HotS [Day 24, 10.08.2024]:
- Knockback dla skilli katany
- Kowal sprzedająca drewnianą broń

HotS [Day 25, 11.08.2024]:
- Dodano siekierkę wraz ze skillem i pasywką
- Wstawiono biolog na mapę
- Zalążek systemu questów (Task.j)

HotS [Day 26, 14.08.2024]:
- Poprawki kodu w TextTag system
- Dokończono system questów
- Działające questy od biolog

HotS [Day 27, 18.08.2024]:
- Teren dżungli
- Nowy mob: Grubby
- Zwiększenie HP mobów o 50%
- Śledzenie pozycji myszki i komenda /tp
- Poprawki skilli Siekierki i Katany

HotS [Day 28, 20.08.2024]:
- Dodano cukier i jego drop z BeFlowerów
- Dodano możliwość stackowania i rozstackowywania niektórych itemów
- Poprawiono animację śmierci kwiatków

HotS [Day 29, 22.08.2024]:
- Dodano deszcz
- Muzyka dla deszczu i dżungli
- Lokalna zmiana muzyki
- Broń nie traci durability gdy celem jest kwiatek
- Zmniejszony spawn rate kwiatków
- Bugfix: podnoszenie grubbych

HotS [Day 30, 28.08.2024]:
- Przygotowania do dodania dual wieldingu w Weapon.j

HotS [Day 31, 29.08.2024]:
- Dodano dual wielding z prawdziwego zdarzenia
- Bugfix: sprzedawanie przedmiotów u handlarki
- Bugfix: worm poprawnie znika po zjedzeniu
- Brutalna siła przywraca dodatkowo (poziom umiejętności) punktów życia

HotS [Day 32, 31.08.2024]:
- Dodano World Starvera
- Eksperymenty z cukrem

Hots [Day 33, 03.09.2024]:
- Opracowanie team colored smugi przy używaniu skilli
- Drugi spell World Starvera: Mark of Starvation

Hots [Day 34, 04.09.2024]:
- Team colored smuga skilli, czapka i hero glow
- Poprawiono knockback, nowa funkcja: KnockbackTarget
- Poprawiono wygląd pocisków World Starvera
- Poprawiono bug z kolcami World Starvera
- Metin sfx
- Poprawiono błędy z ekwipowaniem broni dwuręcznej i dual wieldingiem
- Bardziej szczegółowe opisy broni

Hots [Day 35, 05.09.2024]:
- Dodano Arcing Loot
- Lepszy drop patyków i jabłek z drzew
- Poprawiono komendę /color

HotS [day 36, 09.09.2024]:
- Zalążek systemu craftingu
- Praca nad zmodernizowaną skrzynką/plecakiem

HotS [Day 37, 11.09.2024]:
- Dodano skrzynie graczy (z których można korzystać nawet gdy są schowane)

HotS [Day 38, 11.09.2024]:
- Dokończenie i poprawki błędów systemu craftingu
- Poprawiono zbugowany pasek zdrowia skrzyni

HotS [Day 39, 13.09.2024]:
- Zmienne przechowujące wartości RGB kolorów graczy
- Kolorowe skrzynie
- Lepszy spawn wilków

HotS [Day 40, 15.12.2024]:
- Usunięto śmieci z ziemi (wyder prep)

HotS [Day 41, 02.03.2024]:
- Refactor Announcer.j (ZNACZĄCO bardziej elastyczny)
- Poprawki prio w Audio.j

HotS [Day 42, 03.03.2024]:
- Dodano tick i done eventy do Buff.j

HotS [Day 42, 04.03.2024]:
- Loop.j jako zamiennik Timer.j