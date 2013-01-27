def CalculateMetric(executedAndPassedCount, passedCount, executedAndFailedCount, failedCount):
	
	if passedCount == 0 and failedCount == 0:
		return [float("nan"), float("nan"), float("nan")]
	if passedCount == 0:
		return [executedAndFailedCount / failedCount, float("nan"), executedAndFailedCount / failedCount]
	if failedCount == 0:
		return [0.0, executedAndPassedCount / passedCount, float("nan")]
	
	p = executedAndPassedCount / passedCount
	f = executedAndFailedCount / failedCount
	
	pe = executedAndPassedCount
	fe = executedAndFailedCount
	
	pa = passedCount
	fa = failedCount
	ea = fe + pe

	if (fa + pa) == 0:
		return [float("nan"), p, f]
	
	return [fe / (fa + pa), p, f]
