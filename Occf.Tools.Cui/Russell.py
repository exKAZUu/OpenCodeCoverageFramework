def CalculateMetric(executedAndPassedCount, passedCount, executedAndFailedCount, failedCount):
	if passedCount == 0 and failedCount == 0:
		return [0.0, float("nan"), float("nan")]
	if passedCount == 0:
		return [0.0, float("nan"), executedAndFailedCount / failedCount]
	if failedCount == 0:
		return [3.0, executedAndPassedCount / passedCount, float("nan")]
	p = executedAndPassedCount / passedCount
	f = executedAndFailedCount / failedCount
	pe = executedAndPassedCount
	fe = executedAndFailedCount
	pa = passedCount
	fa = failedCount
	ea = fe + pe
	return [fe / (fa + pa), p, f]
