#include "unity_interface.h"
#include <stdio.h>
#include <string>
#include <unicode/udata.h>
#include <unicode/brkiter.h> // use icu break iterator

static UErrorCode m_result = U_ZERO_ERROR;
static icu::BreakIterator *m_breakIterator = NULL;
static std::string m_locale;
static bool m_loadedData = false;

////////////////////////

// for debug
typedef void (*DebugFuncPtr)(const char *);
DebugFuncPtr Debug;

// for debug
typedef void (*ReturnStringFuncPtr)(const char *, int);
ReturnStringFuncPtr ReturnString;

std::string m_output;

extern "C"
{
	void UNITY_EXPORT ICU4USetDebugFunction(DebugFuncPtr fp) { Debug = fp; }
	void UNITY_EXPORT ICU4USetReturnStringFunction(ReturnStringFuncPtr fp) { ReturnString = fp; }

	bool UNITY_EXPORT ICU4USetICUData(const char *bytes)
	{
		Debug("ICU4U: Setting ICU Data...");
		const char *data = bytes;
		m_result = U_ZERO_ERROR;
		udata_setCommonData(data, &m_result);
		if (U_SUCCESS(m_result))
		{
			m_loadedData = true;
			m_output.reserve(1024);
			Debug("ICU4U: Set ICU data successful");
			return true;
		}
		Debug("ICU4U: Set ICU data was not successful due to:");
		Debug(u_errorName(m_result));
		return false;
	}

	void UNITY_EXPORT ICU4USetICUDataPath(const char *path)
	{
		//Debug("ICU4U: Setting ICU data path to:");
		//Debug(path);
		u_setDataDirectory(path);
		m_loadedData = true;
		m_output.reserve(1024);
	}

	bool UNITY_EXPORT ICU4UIsDataLoaded()
	{
		return m_loadedData;
	}

	bool UNITY_EXPORT ICU4USetLocale(char *newLocale)
	{
		if (!m_loadedData)
			return false;

		if (m_locale != newLocale)
		{
			Debug("ICU4U: Locale is being set to: ");
			Debug(newLocale);

			m_locale = newLocale;

			if (m_breakIterator != NULL)
			{
				delete m_breakIterator;
				m_breakIterator = NULL;
			}

			m_result = U_ZERO_ERROR;
			icu::Locale toUse = icu::Locale(m_locale.c_str());
			m_breakIterator = icu::BreakIterator::createLineInstance(toUse, m_result);

			if (U_SUCCESS(m_result))
			{
				m_loadedData = true;
				Debug("ICU4U: Created BreakIterator successfully");
				return true;
			}
			else
			{
				m_breakIterator = NULL;
				Debug("ICU4U: BreakIterator creation failed.");
				return false;
			}
		}
		else
		{
			return false;
		}
	}

	void UNITY_EXPORT ICU4UInsertLineBreaks(char *text, int reqNo, int breakCharacter)
	{
		if (!m_loadedData)
		{
			Debug("ICU4U: ICU data not loaded");
			return;
		}
		if (m_breakIterator == NULL)
		{
			Debug("ICU4U: BreakIterator not created");
			return;
		}

		//Debug("beginning line break for:");
		//Debug(text);

		// prepare icu::UnicodeString
		icu::UnicodeString icuInput(text);
		icu::UnicodeString icuBreakCharacter;
		icuBreakCharacter.append(breakCharacter);

		// set text for break iterator
		m_breakIterator->setText(icuInput);

		// get words one at a time and put them into the inOutString, with dashes
		int32_t currPos = m_breakIterator->first();
		m_output = "";

		while (currPos < icuInput.length())
		{
			int32_t begin = currPos;
			m_breakIterator->next();
			int32_t end = m_breakIterator->current();
			currPos = end;

			int32_t length = end - begin;

			// extract next word
			icu::UnicodeString icuWord;
			icuInput.extract(begin, length, icuWord);

			// append next word to output
			icuWord.toUTF8String(m_output);

			// if last char wasn't whitespace and we are not at last position, then add the breakCharacter
			if (currPos > 0) {
				int lastChar = icuInput.charAt(currPos-1);
				if (lastChar != ' ' && lastChar != '\n' && lastChar != L'\u200B' && lastChar != breakCharacter && currPos != icuInput.length())
				{
					int nextChar = icuInput.charAt(currPos);
					if (nextChar != breakCharacter) {
						icuBreakCharacter.toUTF8String(m_output);
					}
				}
			}
		}

		//Debug("result is:");
		//Debug(m_output.c_str());

		// sending result back:
		ReturnString(m_output.c_str(), reqNo);
	}
}

// for testing when built as executable
/*

static void print(const char *text)
{
	printf(text);
	printf("\n");
}

static void test(const char *text)
{
	char buffer[1024];
	strcpy(buffer, text);
	int sizeExample = strlen(text) * 2;
	printf("\n");
	printf("input:\n");
	printf(buffer);
	printf("\n");
	ICU4UInsertLineBreaks(buffer, sizeExample, ':');
	printf("result:\n");
	printf(buffer);
}

void main()
{
	Debug = print;
	char *path = "../../../StreamingAssets/icudt63l.dat";

	//*

	// can either use path
	ICU4USetICUDataPath(path);

	/*/

/*
	// or load buffer
	FILE *file = fopen(path, "r");
	fseek(file, 0L, SEEK_END);
	int size = ftell(file);
	fseek(file, 0L, SEEK_SET);

	char *buffer = (char *)malloc(size);
	int read = fread(buffer, 1, size, file);

	printf("loaded %d bytes\n", read);

	ICU4USetICUData(buffer);
	//*/

/*
	ICU4USetLocale("en");
	// english
	test("We are testing word breaking with different languages. This was translated by Google Translate. Sorry if it is weird.");
	// thai
	test("เรากำลังทดสอบการตัดคำภาษาต่างๆ อันนี้แปลโดย Google Translate ขออภัยถ้ามันแปลตลก คือเฉพาะภาษาไทยที่แปลเองครับ");
	// chinese traditional
	test("我們正在測試使用不同語言的單詞。 這是由谷歌翻譯翻譯。 對不起，如果它很奇怪。");
	// chinese simplified
	test("我们正在测试使用不同语言的单词。 这是由谷歌翻译翻译。 对不起，如果它很奇怪。");
	// japanese
	test("私たちは、異なる言語で単語を壊すことをテストしています。 これはGoogle翻訳で翻訳されました。 それが奇妙な場合は申し訳ありません。");
}*/