---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/reporter.ts"
ref: HEAD
---

export function createReporter(
  reportStep: number,
  report: (sum: number) => void
) {
  let sum = 0;
  let nextReportSum = reportStep;

  function incr(num: number) {
    sum += num;

    if (sum >= nextReportSum) {
      nextReportSum += reportStep;

      report(sum);
    }
  }

  return {
    incr,
  };
}
