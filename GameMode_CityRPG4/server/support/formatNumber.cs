// strFormatNumber(str)
// This is a quick and dirty method to format cash strings.
// Returns a string.
function strFormatNumber(%str)
{
	// Negative value handling
	// Here we're taking advantage of the fact that literally everything is a string in Torque.
	%tempA = strreplace(%str, "-", "");

	if(%tempA !$= %str)
		%negative = true;

	// Assign the string length.
	%len = strlen(%tempA);

	// Only strings up to 10 chars for now -- also, skip unnecessary strings
	if(%len > 9 || %len < 4)
		return %str;

	%tempB = "";
	%result = "";

	// Format separators into the string.
	for(%i = 0; %i <= %len-1; %i++)
	{
		if(%i != 0 && %i % 3 == 0)
			%tempB = %tempB @ "," @ getSubStr(%tempA, %len-%i-1, 1);
		else
			%tempB = %tempB @ getSubStr(%tempA, %len-%i-1, 1);
	}

	// Reverse the string.
	for(%i = 0; %i <= %len+1; %i++)
		%result = getSubStr(%tempB, %i, 1) @ %result;

	if(%negative)
		%result = "-" @ %result;

	return %result;
}
