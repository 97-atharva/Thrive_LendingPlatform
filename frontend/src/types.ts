export type Decision = {
  applicationId: string;
  status: "Successful" | "Declined";
  loanToValuePercent: number;
  reason: string;
  submittedAt: string;
};

export type Portfolio = {
  totalApplications: number;
  successfulApplications: number;
  declinedApplications: number;
  totalLoansWritten: number;
  meanLoanToValuePercent: number;
};